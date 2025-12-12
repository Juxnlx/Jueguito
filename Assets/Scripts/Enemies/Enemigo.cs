using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public Sprite[] spritesAnimacion;
    public float tiempoAnimacion = 1f;

    private SpriteRenderer renderizadorSprite;
    private int indiceAnimacion;

    public int columna;

    // Tipo de enemigo
    public enum TipoEnemigo { Seta, Minotauro }
    public TipoEnemigo tipo;

    // Para el ataque del minotauro
    private bool estaAtacando = false;
    private float tiempoEntreAtaques = 1.5f;
    private float ultimoAtaque = 0f;

    private void Awake()
    {
        renderizadorSprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimarSprite), tiempoAnimacion, tiempoAnimacion);
    }

    private void AnimarSprite()
    {
        indiceAnimacion++;
        if (indiceAnimacion >= spritesAnimacion.Length)
            indiceAnimacion = 0;

        renderizadorSprite.sprite = spritesAnimacion[indiceAnimacion];
    }

    private void Update()
    {
        // Solo los minotauros atacan cuerpo a cuerpo
        if (tipo == TipoEnemigo.Minotauro && estaAtacando)
        {
            if (Time.time > ultimoAtaque + tiempoEntreAtaques)
            {
                AtacarConHacha();
                ultimoAtaque = Time.time;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si un minotauro toca una base, empieza a atacar
        if (tipo == TipoEnemigo.Minotauro && other.GetComponent<Base>() != null)
        {
            estaAtacando = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Si el minotauro se aleja de la base, deja de atacar
        if (tipo == TipoEnemigo.Minotauro && other.GetComponent<Base>() != null)
        {
            estaAtacando = false;
        }
    }

    private void AtacarConHacha()
    {
        // Buscar bases cercanas en un radio
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D col in colisiones)
        {
            Base baseObj = col.GetComponent<Base>();
            if (baseObj != null)
            {
                // Daño directo a la base
                baseObj.RecibirDanio(2); // El hacha hace más daño
                break;
            }
        }
    }

    public bool PuedoDisparar()
    {
        // Solo las setas disparan
        if (tipo != TipoEnemigo.Seta)
            return false;

        // Verificar si hay algún enemigo delante (hacia la izquierda)
        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor == null)
            return false;

        return gestor.NoHayEnemigoDelante(this);
    }

    private void OnDestroy()
    {
        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor != null)
        {
            gestor.ActualizarEnemigosFrontales();
        }
    }
}