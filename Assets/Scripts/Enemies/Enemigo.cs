using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public Sprite[] spritesAnimacion;
    public float tiempoAnimacion = 1f;
    private SpriteRenderer renderizadorSprite;
    private int indiceAnimacion;
    public int columna;

    public enum TipoEnemigo { Seta, Minotauro }
    public TipoEnemigo tipo;

    private bool estaAtacando = false;
    private float tiempoEntreAtaques = 1.5f;
    private float ultimoAtaque = 0f;
    private bool avanzandoRapido = false;

    private Animator animator;
    private bool estaMuriendo = false;

    [SerializeField] private float velocidadAvanceRapido = 3f;
    private Rigidbody2D rb2D;
    private bool baseDestruidaEnContacto = false;

    private void Awake()
    {
        renderizadorSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
            rb2D.gravityScale = 0;
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimarSprite), tiempoAnimacion, tiempoAnimacion);
    }

    private void AnimarSprite()
    {
        if (estaMuriendo) return;

        indiceAnimacion++;
        if (indiceAnimacion >= spritesAnimacion.Length)
            indiceAnimacion = 0;
        renderizadorSprite.sprite = spritesAnimacion[indiceAnimacion];
    }

    private void Update()
    {
        if (estaMuriendo) return;

        if (tipo == TipoEnemigo.Minotauro)
        {
            if (estaAtacando)
            {
                if (Time.time > ultimoAtaque + tiempoEntreAtaques)
                {
                    AtacarConHacha();
                    ultimoAtaque = Time.time;
                }
            }
            else if (avanzandoRapido)
            {
                AvanzarHaciaBases();
            }
        }
    }

    private void AvanzarHaciaBases()
    {
        transform.position += Vector3.left * velocidadAvanceRapido * Time.deltaTime;

        if (animator != null)
        {
            animator.SetBool("Corriendo", true);
        }
    }

    public void ActivarAvanceRapido(bool activar)
    {
        avanzandoRapido = activar;

        if (activar && tipo == TipoEnemigo.Minotauro)
        {
            transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (estaMuriendo) return;

        if (tipo == TipoEnemigo.Minotauro)
        {
            Base baseObj = other.GetComponent<Base>();
            if (baseObj != null)
            {
                estaAtacando = true;
                avanzandoRapido = false;
                baseDestruidaEnContacto = false;

                if (rb2D != null)
                {
                    rb2D.velocity = Vector2.zero;
                }
            }

            MonoBehaviourBlobbie jugador = other.GetComponent<MonoBehaviourBlobbie>();
            if (jugador != null)
            {
                jugador.RecibirDanio(1);
                Morir();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (tipo == TipoEnemigo.Minotauro)
        {
            Base baseObj = other.GetComponent<Base>();
            if (baseObj != null)
            {
                estaAtacando = false;
                
                if (baseDestruidaEnContacto)
                {
                    avanzandoRapido = true;
                }
            }
        }
    }

    private void AtacarConHacha()
    {
        if (animator != null)
        {
            animator.SetTrigger("Atacar");
        }

        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, 0.8f);
        foreach (Collider2D col in colisiones)
        {
            Base baseObj = col.GetComponent<Base>();
            if (baseObj != null)
            {
                baseObj.RecibirDanio(1);
                baseDestruidaEnContacto = true;
                break;
            }
        }
    }

    public bool PuedoDisparar()
    {
        if (estaMuriendo) return false;

        if (tipo != TipoEnemigo.Seta)
            return false;

        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor == null)
            return false;

        return gestor.NoHayEnemigoDelante(this);
    }

    public void Atacar()
    {
        if (estaMuriendo) return;

        if (animator != null)
        {
            animator.SetTrigger("Atacar");
        }
    }

    public void DispararProyectil()
    {
        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor != null)
        {
            gestor.CrearBalaDesdeEnemigo(this);
        }
    }

    public void Morir()
    {
        if (estaMuriendo) return;

        estaMuriendo = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetTrigger("Morir");
        }

        Destroy(gameObject, 0.8f);
    }

    private void OnDestroy()
    {
        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor != null)
        {
            gestor.ActualizarEnemigosFrontales();
            gestor.VerificarMinotaurosLibres();
        }
    }
}