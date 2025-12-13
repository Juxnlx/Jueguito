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

    // NUEVO: Para controlar la muerte
    private Animator animator;
    private bool estaMuriendo = false;

    private void Awake()
    {
        renderizadorSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // NUEVO: Obtener el Animator
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimarSprite), tiempoAnimacion, tiempoAnimacion);
    }

    private void AnimarSprite()
    {
        // NUEVO: No animar si está muriendo
        if (estaMuriendo) return;

        indiceAnimacion++;
        if (indiceAnimacion >= spritesAnimacion.Length)
            indiceAnimacion = 0;
        renderizadorSprite.sprite = spritesAnimacion[indiceAnimacion];
    }

    private void Update()
    {
        // NUEVO: No hacer nada si está muriendo
        if (estaMuriendo) return;

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
        // NUEVO: No detectar colisiones si está muriendo
        if (estaMuriendo) return;

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
        // NUEVO: No disparar si está muriendo
        if (estaMuriendo) return false;

        // Solo las setas disparan
        if (tipo != TipoEnemigo.Seta)
            return false;

        // Verificar si hay algún enemigo delante (hacia la izquierda)
        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor == null)
            return false;

        return gestor.NoHayEnemigoDelante(this);
    }

    // Método para activar la animación de ataque
    public void Atacar()
    {
        if (estaMuriendo) return; // No atacar si está muriendo

        // Activar el trigger de ataque en el Animator
        if (animator != null)
        {
            animator.SetTrigger("Atacar");
        }
    }

    // NUEVO: Método público para matar al enemigo con animación
    public void Morir()
    {
        if (estaMuriendo) return; // Ya está muriendo, no hacer nada

        estaMuriendo = true;

        // Desactivar el collider para que no colisione más
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Activar el trigger de muerte en el Animator
        if (animator != null)
        {
            animator.SetTrigger("Morir");
        }

        // Destruir el enemigo después de la animación (ajusta el tiempo según tu animación)
        Destroy(gameObject, 0.6f); // 0.6 segundos, ajusta según la duración de tu animación
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