using System.Collections.Generic;
using UnityEngine;

public class Enemigos : MonoBehaviour
{
    // ===== PREFABS DE ENEMIGOS =====
    public Enemigo prefabM; // Prefab del Minotauro
    public Enemigo prefabS; // Prefab de la Seta

    // ===== CONFIGURACIÓN DE LA HORDA =====
    public int filas = 4; // Número de filas de enemigos
    public int columnas = 5; // Número de columnas de enemigos

    // ===== CONFIGURACIÓN DE SEPARACIÓN =====
    public float separacionX = 0.1f; // Separación horizontal entre enemigos
    public float separacionY = 0.35f; // Separación vertical entre enemigos

    // ===== CONFIGURACIÓN DE MOVIMIENTO =====
    public float velocidad = 0.7f; // Velocidad de movimiento vertical
    public float pasoIzquierda = 0.2f; // Cuánto avanzan hacia la izquierda al cambiar dirección
    public float margenArbustos = 0.1f; // Margen superior e inferior

    // ===== CONFIGURACIÓN DE POSICIÓN INICIAL =====
    [Tooltip("Posición inicial en X como % del ancho de pantalla (1.5 = fuera de cámara a la derecha)")]
    public float posicionInicialX = 1.5f; // MODIFICADO: Ahora aparecen FUERA de la cámara
    [Tooltip("Desplazamiento vertical adicional en unidades del mundo")]
    public float desplazamientoVertical = 1f;

    // ===== CONFIGURACIÓN DE LÍMITES =====
    [Tooltip("Límite en X que no pueden sobrepasar (donde están las bases)")]
    public float limiteDerecho = 0.3f;

    // ===== CONFIGURACIÓN DE DISPAROS =====
    public GameObject prefabBalaEnemigo; // Prefab de la bala enemiga
    public float tiempoMinimoEntreDisparos = 1f; // Tiempo mínimo entre disparos
    public float tiempoMaximoEntreDisparos = 3f; // Tiempo máximo entre disparos
    private float proximoDisparo = 0f; // Timestamp del próximo disparo

    // ===== CONTROL DE MOVIMIENTO =====
    private bool subiendo = true; // Dirección actual del movimiento vertical
    private float limiteSuperior; // Límite superior de movimiento
    private float limiteInferior; // Límite inferior de movimiento

    // ===== SISTEMA DE ENEMIGOS FRONTALES =====
    // Diccionario que guarda el enemigo más adelantado de cada columna
    private Dictionary<int, Enemigo> enemigosFrontales;

    private void Awake()
    {
        // ===== CALCULAR SEPARACIÓN AUTOMÁTICA SEGÚN SPRITES =====
        float anchoMax = Mathf.Max(
            prefabM.GetComponent<SpriteRenderer>().bounds.size.x,
            prefabS.GetComponent<SpriteRenderer>().bounds.size.x
        );
        float altoMax = Mathf.Max(
            prefabM.GetComponent<SpriteRenderer>().bounds.size.y,
            prefabS.GetComponent<SpriteRenderer>().bounds.size.y
        );
        float margen = 0.05f;
        separacionX = anchoMax + margen;
        separacionY = altoMax + margen;

        // ===== CALCULAR LÍMITES DE MOVIMIENTO =====
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        float altoEnemigo = altoMax;
        limiteInferior = bottomLeft.y + altoEnemigo / 2 + margenArbustos;
        limiteSuperior = topRight.y - altoEnemigo / 2 - margenArbustos;

        // ===== CALCULAR POSICIÓN INICIAL (FUERA DE CÁMARA) =====
        float anchoJuego = topRight.x - bottomLeft.x;

        // MODIFICADO: Posición inicial FUERA de la cámara a la derecha
        // Si posicionInicialX > 1.0, los enemigos aparecen fuera de la vista
        Vector3 startPos = new Vector3(
            bottomLeft.x + anchoJuego * posicionInicialX, // 1.5 = 150% del ancho = fuera
            (limiteSuperior + limiteInferior) / 2f + desplazamientoVertical,
            0
        );

        // ===== CREAR LA HORDA DE ENEMIGOS =====
        int numColumnasM = 2; // Primeras 2 columnas son minotauros (atrás)

        for (int fila = 0; fila < filas; fila++)
        {
            // Calcular posición base de esta fila
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

                // Instanciar enemigo como hijo de este GameObject
                Enemigo enemigo = Instantiate(prefabElegido, this.transform);
                enemigo.columna = col;

                // Asignar tipo de enemigo
                enemigo.tipo = esMinotauro ? Enemigo.TipoEnemigo.Minotauro : Enemigo.TipoEnemigo.Seta;

                // Calcular posición específica de este enemigo
                Vector3 posicion = posicionFila;
                posicion.x -= col * separacionX; // Columnas más altas están más a la izquierda
                enemigo.transform.position = posicion;
            }
        }

        // Actualizar el sistema de enemigos frontales
        ActualizarEnemigosFrontales();

        // Inicializar el primer disparo con tiempo aleatorio
        proximoDisparo = Time.time + Random.Range(tiempoMinimoEntreDisparos, tiempoMaximoEntreDisparos);
    }

    private void Update()
    {
        // ===== MOVIMIENTO VERTICAL =====
        float dirY = subiendo ? 1 : -1;

        // Solo mover si no han llegado al límite derecho (cerca de las bases)
        if (transform.position.x > limiteDerecho)
        {
            transform.position += Vector3.up * dirY * velocidad * Time.deltaTime;
        }

        // ===== VERIFICAR LÍMITES Y CAMBIAR DIRECCIÓN =====
        foreach (Transform enemigo in transform)
        {
            if (!enemigo.gameObject.activeInHierarchy) continue;

            // Verificar si algún enemigo tocó el límite inferior
            if (!subiendo && enemigo.position.y <= limiteInferior)
            {
                CambiarDireccion();
                break;
            }
            // Verificar si algún enemigo tocó el límite superior
            else if (subiendo && enemigo.position.y >= limiteSuperior)
            {
                CambiarDireccion();
                break;
            }
        }

        // ===== SISTEMA DE DISPAROS ALEATORIOS =====
        if (Time.time >= proximoDisparo)
        {
            IniciarAtaqueSeta();
            float tiempoAleatorio = Random.Range(tiempoMinimoEntreDisparos, tiempoMaximoEntreDisparos);
            proximoDisparo = Time.time + tiempoAleatorio;
        }
    }

    // ===== CAMBIAR DIRECCIÓN Y AVANZAR =====
    private void CambiarDireccion()
    {
        // Invertir dirección vertical
        subiendo = !subiendo;

        // Solo avanzar hacia la derecha (hacia las bases) si no han llegado al límite
        if (transform.position.x > limiteDerecho)
        {
            Vector3 pos = transform.position;
            pos.x -= pasoIzquierda; // Avanzar hacia la izquierda
            transform.position = pos;
        }
    }

    // ===== ACTUALIZAR LISTA DE ENEMIGOS FRONTALES =====
    public void ActualizarEnemigosFrontales()
    {
        enemigosFrontales = new Dictionary<int, Enemigo>();

        // Para cada columna, encontrar el enemigo más adelantado (más a la izquierda)
        for (int col = 0; col < columnas; col++)
        {
            Enemigo frontal = null;

            foreach (Transform enemigoT in transform)
            {
                Enemigo e = enemigoT.GetComponent<Enemigo>();
                if (e == null || !e.gameObject.activeInHierarchy) continue;
                if (e.columna != col) continue;

                // El más adelantado es el que tiene menor X
                if (frontal == null || e.transform.position.x < frontal.transform.position.x)
                {
                    frontal = e;
                }
            }

            // Guardar el enemigo frontal de esta columna
            if (frontal != null)
            {
                enemigosFrontales[col] = frontal;
            }
        }
    }

    // ===== INICIAR ANIMACIÓN DE ATAQUE DE UNA SETA =====
    private void IniciarAtaqueSeta()
    {
        // Crear lista de setas que pueden disparar
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

            // MODIFICADO: Solo activar la animación, NO crear la bala aún
            // La bala se creará desde un Animation Event en el frame correcto
            setaElegida.Atacar();
        }
    }

    // ===== CREAR BALA DESDE UN ENEMIGO (LLAMADO POR ANIMATION EVENT) =====
    public void CrearBalaDesdeEnemigo(Enemigo enemigo)
    {
        // Verificar que el enemigo aún existe y está activo
        if (enemigo == null || !enemigo.gameObject.activeInHierarchy) return;

        // Crear la bala ligeramente a la izquierda del enemigo
        Instantiate(
            prefabBalaEnemigo,
            enemigo.transform.position + Vector3.left * 0.1f,
            Quaternion.identity
        );
    }

    // ===== VERIFICAR SI HAY ENEMIGO DELANTE =====
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

    // ===== VERIFICAR MINOTAUROS CON VÍA LIBRE =====
    public void VerificarMinotaurosLibres()
    {
        // Buscar todos los minotauros activos
        foreach (Transform enemigoT in transform)
        {
            if (!enemigoT.gameObject.activeInHierarchy) continue;

            Enemigo e = enemigoT.GetComponent<Enemigo>();
            if (e == null || e.tipo != Enemigo.TipoEnemigo.Minotauro) continue;

            // Si no hay nadie delante, activar avance rápido
            bool viaLibre = NoHayEnemigoDelante(e);
            e.ActivarAvanceRapido(viaLibre);
        }
    }
}