using System.Collections.Generic;
using UnityEngine;

public class Enemigos : MonoBehaviour
{
    public Enemigo prefabM; // Minotauro
    public Enemigo prefabS; // Seta
    public int filas = 4;
    public int columnas = 5;

    public float separacionX = 0.1f;
    public float separacionY = 0.35f;
    public float velocidad = 0.7f;
    public float pasoIzquierda = 0.2f;
    public float margenArbustos = 0.1f;
    public float posicionInicialX = 0.95f;
    public float limiteDerecho = 0.3f; // Límite en X que no pueden sobrepasar (ajústalo según dónde estén tus bases)

    public GameObject prefabBalaEnemigo;
    public float tiempoMinimoEntreDisparos = 1f; // Tiempo mínimo entre disparos
    public float tiempoMaximoEntreDisparos = 3f; // Tiempo máximo entre disparos
    private float proximoDisparo = 0f;

    private bool subiendo = true;
    private float limiteSuperior;
    private float limiteInferior;

    private Dictionary<int, Enemigo> enemigosFrontales;

    private void Awake()
    {
        float anchoMax = Mathf.Max(prefabM.GetComponent<SpriteRenderer>().bounds.size.x,
                                   prefabS.GetComponent<SpriteRenderer>().bounds.size.x);
        float altoMax = Mathf.Max(prefabM.GetComponent<SpriteRenderer>().bounds.size.y,
                                  prefabS.GetComponent<SpriteRenderer>().bounds.size.y);
        float margen = 0.05f;
        separacionX = anchoMax + margen;
        separacionY = altoMax + margen;

        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        float altoEnemigo = altoMax;
        limiteInferior = bottomLeft.y + altoEnemigo / 2 + margenArbustos;
        limiteSuperior = topRight.y - altoEnemigo / 2 - margenArbustos;

        float anchoJuego = topRight.x - bottomLeft.x;
        // MODIFICADO: Posición inicial más clara
        Vector3 startPos = new Vector3(
            bottomLeft.x + anchoJuego * posicionInicialX,
            (limiteSuperior + limiteInferior) / 2f + 1f, // El +1f los sube 1 unidad
            0
        );

        int numColumnasM = 2; // primeras 2 columnas → minotauros (atrás)
        for (int fila = 0; fila < filas; fila++)
        {
            Vector3 posicionFila = new Vector3(
                startPos.x,
                startPos.y - fila * separacionY,
                0
            );

            for (int col = 0; col < columnas; col++)
            {
                // Las primeras columnas son minotauros (atrás), las últimas setas (al frente)
                bool esMinotauro = col < numColumnasM;
                Enemigo prefabElegido = esMinotauro ? prefabM : prefabS;
                Enemigo enemigo = Instantiate(prefabElegido, this.transform);
                enemigo.columna = col;

                // Asignar tipo
                enemigo.tipo = esMinotauro ? Enemigo.TipoEnemigo.Minotauro : Enemigo.TipoEnemigo.Seta;

                Vector3 posicion = posicionFila;
                posicion.x -= col * separacionX;
                enemigo.transform.position = posicion;
            }
        }

        ActualizarEnemigosFrontales();

        // NUEVO: Inicializar el primer disparo con tiempo aleatorio
        proximoDisparo = Time.time + Random.Range(tiempoMinimoEntreDisparos, tiempoMaximoEntreDisparos);
    }

    private void Update()
    {
        float dirY = subiendo ? 1 : -1;

        // NUEVO: Solo mover si no han llegado al límite derecho
        if (transform.position.x > limiteDerecho)
        {
            transform.position += Vector3.up * dirY * velocidad * Time.deltaTime;
        }

        foreach (Transform enemigo in transform)
        {
            if (!enemigo.gameObject.activeInHierarchy) continue;

            if (!subiendo && enemigo.position.y <= limiteInferior)
            {
                CambiarDireccion();
                break;
            }
            else if (subiendo && enemigo.position.y >= limiteSuperior)
            {
                CambiarDireccion();
                break;
            }
        }

        // Disparo con tiempo aleatorio
        if (Time.time >= proximoDisparo)
        {
            DispararSetaAleatoria();
            float tiempoAleatorio = Random.Range(tiempoMinimoEntreDisparos, tiempoMaximoEntreDisparos);
            proximoDisparo = Time.time + tiempoAleatorio;
        }
    }

    private void CambiarDireccion()
    {
        subiendo = !subiendo;

        // NUEVO: Solo avanzar hacia la derecha si no han llegado al límite
        if (transform.position.x > limiteDerecho)
        {
            Vector3 pos = transform.position;
            pos.x -= pasoIzquierda;
            transform.position = pos;
        }
    }

    public void ActualizarEnemigosFrontales()
    {
        enemigosFrontales = new Dictionary<int, Enemigo>();

        for (int col = 0; col < columnas; col++)
        {
            Enemigo frontal = null;

            foreach (Transform enemigoT in transform)
            {
                Enemigo e = enemigoT.GetComponent<Enemigo>();
                if (e == null || !e.gameObject.activeInHierarchy) continue;
                if (e.columna != col) continue;

                if (frontal == null || e.transform.position.x < frontal.transform.position.x)
                {
                    frontal = e;
                }
            }

            if (frontal != null)
            {
                enemigosFrontales[col] = frontal;
            }
        }
    }

    private void DispararSetaAleatoria()
    {
        // Crear una lista de setas que pueden disparar
        List<Enemigo> setasDisponibles = new List<Enemigo>();

        foreach (Transform enemigoT in transform)
        {
            if (!enemigoT.gameObject.activeInHierarchy) continue;

            Enemigo e = enemigoT.GetComponent<Enemigo>();
            if (e != null && e.PuedoDisparar())
            {
                setasDisponibles.Add(e);
            }
        }

        // Si hay setas disponibles, elegir UNA al azar
        if (setasDisponibles.Count > 0)
        {
            int indiceAleatorio = Random.Range(0, setasDisponibles.Count);
            Enemigo setaElegida = setasDisponibles[indiceAleatorio];

            Debug.Log("Disparando seta. Total disponibles: " + setasDisponibles.Count); // NUEVO

            // Activar la animación de ataque
            setaElegida.Atacar();

            // Crear la bala
            Instantiate(prefabBalaEnemigo, setaElegida.transform.position + Vector3.left * 0.1f, Quaternion.identity);
        }
    }

    public bool NoHayEnemigoDelante(Enemigo enemigo)
    {
        // Verificar si hay algún enemigo más a la izquierda (delante) en la misma fila
        float posY = enemigo.transform.position.y;
        float posX = enemigo.transform.position.x;

        foreach (Transform enemigoT in transform)
        {
            if (!enemigoT.gameObject.activeInHierarchy) continue;
            if (enemigoT.gameObject == enemigo.gameObject) continue;

            Enemigo otro = enemigoT.GetComponent<Enemigo>();
            if (otro == null) continue;

            // Si está en la misma fila (con margen de error) y más a la izquierda
            if (Mathf.Abs(otro.transform.position.y - posY) < separacionY * 0.5f &&
                otro.transform.position.x < posX)
            {
                return false; // Hay alguien delante
            }
        }

        return true; // No hay nadie delante
    }
}