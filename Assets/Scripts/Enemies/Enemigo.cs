using UnityEngine;

public class Enemigo : MonoBehaviour
{
    // ===== CONFIGURACIÓN DE ANIMACIÓN SPRITE =====
    public Sprite[] spritesAnimacion; // Array de sprites para animación manual
    public float tiempoAnimacion = 1f; // Tiempo entre cambios de sprite
    private SpriteRenderer renderizadorSprite; // Componente para mostrar sprites
    private int indiceAnimacion; // Índice actual del sprite

    // ===== IDENTIFICACIÓN =====
    public int columna; // Columna en la que se encuentra el enemigo

    // ===== TIPO DE ENEMIGO =====
    public enum TipoEnemigo { Seta, Minotauro }
    public TipoEnemigo tipo; // Tipo actual del enemigo

    // ===== SISTEMA DE ATAQUE DEL MINOTAURO =====
    private bool estaAtacando = false; // Si está en rango de ataque cuerpo a cuerpo
    private float tiempoEntreAtaques = 1.5f; // Cadencia de ataques con hacha
    private float ultimoAtaque = 0f; // Timestamp del último ataque
    private bool avanzandoRapido = false; // Si el minotauro está corriendo hacia las bases

    // ===== SISTEMA DE MUERTE =====
    private Animator animator; // Controlador de animaciones
    private bool estaMuriendo = false; // Flag para evitar múltiples muertes

    // ===== CONFIGURACIÓN DE MOVIMIENTO RÁPIDO =====
    [SerializeField] private float velocidadAvanceRapido = 3f; // Velocidad cuando corre hacia las bases
    private Rigidbody2D rb2D; // Para mover al minotauro independientemente
    private Vector3 posicionInicialEnGrupo; // Para recordar su posición relativa

    private void Awake()
    {
        // Inicializar componentes
        renderizadorSprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        // Si no tiene Rigidbody2D, agregarlo para movimiento independiente
        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
            rb2D.gravityScale = 0; // Sin gravedad
            rb2D.constraints = RigidbodyConstraints2D.FreezeRotation; // No rotar
        }

        // Guardar posición inicial relativa al grupo
        posicionInicialEnGrupo = transform.localPosition;
    }

    private void Start()
    {
        // Iniciar animación de sprites (para enemigos sin Animator complejo)
        InvokeRepeating(nameof(AnimarSprite), tiempoAnimacion, tiempoAnimacion);
    }

    // ===== ANIMACIÓN MANUAL DE SPRITES =====
    private void AnimarSprite()
    {
        // No animar si está muriendo
        if (estaMuriendo) return;

        // Ciclar entre sprites
        indiceAnimacion++;
        if (indiceAnimacion >= spritesAnimacion.Length)
            indiceAnimacion = 0;
        renderizadorSprite.sprite = spritesAnimacion[indiceAnimacion];
    }

    private void Update()
    {
        // No hacer nada si está muriendo
        if (estaMuriendo) return;

        // ===== LÓGICA ESPECÍFICA DE MINOTAUROS =====
        if (tipo == TipoEnemigo.Minotauro)
        {
            // Si está atacando una base cuerpo a cuerpo
            if (estaAtacando)
            {
                // Sistema de ataques periódicos
                if (Time.time > ultimoAtaque + tiempoEntreAtaques)
                {
                    AtacarConHacha();
                    ultimoAtaque = Time.time;
                }
            }
            // Si tiene vía libre pero aún no está atacando, avanzar rápido
            else if (avanzandoRapido)
            {
                AvanzarHaciaBases();
            }
        }
    }

    // ===== MOVIMIENTO RÁPIDO DEL MINOTAURO HACIA LAS BASES =====
    private void AvanzarHaciaBases()
    {
        // Mover hacia la izquierda (donde están las bases) más rápido que el grupo
        transform.position += Vector3.left * velocidadAvanceRapido * Time.deltaTime;

        // Activar animación de correr si existe
        if (animator != null)
        {
            animator.SetBool("Corriendo", true); // Asegúrate de tener este parámetro
        }
    }

    // ===== MÉTODO PÚBLICO PARA VERIFICAR SI PUEDE AVANZAR RÁPIDO =====
    public void ActivarAvanceRapido(bool activar)
    {
        avanzandoRapido = activar;

        // Si se activa el avance rápido, desvincularse del movimiento del grupo
        if (activar && tipo == TipoEnemigo.Minotauro)
        {
            // El minotauro se moverá independientemente
            transform.SetParent(null); // Desvincularse del grupo Enemigos
        }
    }

    // ===== DETECCIÓN DE COLISIONES =====
    private void OnTriggerEnter2D(Collider2D other)
    {
        // No detectar colisiones si está muriendo
        if (estaMuriendo) return;

        // Si un minotauro toca una base, empieza a atacar
        if (tipo == TipoEnemigo.Minotauro && other.GetComponent<Base>() != null)
        {
            estaAtacando = true;
            avanzandoRapido = false; // Ya no avanzar, está en posición de ataque

            // Detener movimiento
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
            }
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

    // ===== ATAQUE CON HACHA DEL MINOTAURO =====
    private void AtacarConHacha()
    {
        // Activar animación de ataque con hacha
        if (animator != null)
        {
            animator.SetTrigger("Atacar"); // Asegúrate de tener este trigger
        }

        // Buscar bases cercanas en un radio
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(transform.position, 0.8f);
        foreach (Collider2D col in colisiones)
        {
            Base baseObj = col.GetComponent<Base>();
            if (baseObj != null)
            {
                // Daño directo a la base (el hacha hace más daño que las balas)
                baseObj.RecibirDanio(2);
                break; // Solo atacar una base por golpe
            }
        }
    }

    // ===== VERIFICAR SI PUEDE DISPARAR (SOLO SETAS) =====
    public bool PuedoDisparar()
    {
        // No disparar si está muriendo
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

    // ===== MÉTODO PARA ACTIVAR LA ANIMACIÓN DE ATAQUE (DISPARO DE SETAS) =====
    public void Atacar()
    {
        // No atacar si está muriendo
        if (estaMuriendo) return;

        // Activar el trigger de ataque en el Animator
        if (animator != null)
        {
            animator.SetTrigger("Atacar"); // Este trigger debe activar la animación de disparo
        }
    }

    // ===== MÉTODO PARA DISPARAR (LLAMADO POR EVENTO DE ANIMACIÓN) =====
    // Este método debe ser llamado en un Animation Event en el frame correcto de la animación de ataque
    public void DispararProyectil()
    {
        // Este método será llamado desde un Animation Event
        // El gestor Enemigos debe manejar la creación real de la bala
        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor != null)
        {
            gestor.CrearBalaDesdeEnemigo(this);
        }
    }

    // ===== MÉTODO DE MUERTE CON ANIMACIÓN =====
    public void Morir()
    {
        // Ya está muriendo, no hacer nada
        if (estaMuriendo) return;

        estaMuriendo = true;

        // Desactivar el collider para que no colisione más
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Detener movimiento
        if (rb2D != null)
        {
            rb2D.velocity = Vector2.zero;
        }

        // Activar el trigger de muerte en el Animator
        if (animator != null)
        {
            animator.SetTrigger("Morir"); // Asegúrate de tener este trigger
        }

        // Destruir el enemigo después de la animación (ajusta el tiempo según tu animación)
        Destroy(gameObject, 0.8f); // Ajusta según la duración de tu animación de muerte
    }

    // ===== EVENTO AL DESTRUIRSE =====
    private void OnDestroy()
    {
        // Notificar al gestor que este enemigo fue eliminado
        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor != null)
        {
            gestor.ActualizarEnemigosFrontales();
            gestor.VerificarMinotaurosLibres(); // Verificar si algún minotauro quedó libre
        }
    }
}