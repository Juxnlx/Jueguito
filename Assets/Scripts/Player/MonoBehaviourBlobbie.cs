using UnityEngine;

public class MonoBehaviourBlobbie : MonoBehaviour
{
    // ===== CONFIGURACIÓN DE MOVIMIENTO =====
    private float vertical; // Input del jugador (arriba/abajo)
    [SerializeField] private float speed; // Velocidad de movimiento
    private Rigidbody2D rb2D; // Componente de física

    // ===== CONFIGURACIÓN DE ANIMACIÓN =====
    private Animator animator; // Controlador de animaciones

    // ===== CONFIGURACIÓN DE DISPARO =====
    public GameObject prefabBala; // Prefab de la bala a disparar
    private float ultimoDisparo = 0; // Tiempo del último disparo
    [SerializeField] private float cadenciaDisparo = 0.5f; // Tiempo entre disparos

    // ===== CONFIGURACIÓN DE VIDA =====
    [SerializeField] private int vidasMaximas = 4; // Máximo de vidas del jugador
    private int vidasActuales; // Vidas actuales
    private bool estaMuriendo = false; // Flag para evitar múltiples muertes
    private bool estaRecibiendo = false; // Flag para evitar daño múltiple simultáneo

    // ===== REFERENCIA A LA UI DE VIDA =====
    private HealthUI healthUI; // Referencia al sistema de corazones

    public AudioClip sonidoDisparo; // Arrastra aquí tu efecto de sonido
    private AudioSource audioSource;

    void Start()
    {
        // Inicializar componentes
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = 5;

        // Inicializar vidas
        vidasActuales = vidasMaximas;

        // Buscar el sistema de UI de vida en la escena
        healthUI = FindObjectOfType<HealthUI>();

        // Asegurar que el collider es un trigger para detectar balas enemigas
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        // No permitir acciones si está muriendo
        if (estaMuriendo) return;

        // Capturar input de movimiento vertical
        vertical = Input.GetAxisRaw("Vertical");

        // Actualizar animación de correr
        animator.SetBool("Run", vertical != 0.0f);

        // Sistema de disparo con cadencia
        if (Input.GetKey(KeyCode.Space) && Time.time > ultimoDisparo + cadenciaDisparo)
        {
            Shoot();
            ultimoDisparo = Time.time;
        }
    }

    private void FixedUpdate()
    {
        // No mover si está muriendo
        if (estaMuriendo) return;

        // Aplicar movimiento vertical
        rb2D.velocity = new Vector2(0, vertical * speed);
    }

    // ===== MÉTODO DE DISPARO =====
    private void Shoot()
    {
        GameObject bala = Instantiate(
            prefabBala,
            transform.position + Vector3.right * 0.1f,
            Quaternion.identity
        );

        Destroy(bala, 1f);

        // ===== Reproducir sonido de disparo =====
        if (sonidoDisparo != null)
        {
            AudioSource.PlayClipAtPoint(sonidoDisparo, transform.position, 1f);
        }
    }

    // ===== SISTEMA DE DAÑO =====
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignorar colisiones si está muriendo o ya recibiendo daño
        if (estaMuriendo || estaRecibiendo) return;

        // Detectar si es una bala enemiga
        BalaEnemigo balaEnemiga = other.GetComponent<BalaEnemigo>();
        if (balaEnemiga != null)
        {
            // Recibir daño
            RecibirDanio(1);

            // Destruir la bala enemiga
            Destroy(other.gameObject);
        }
    }

    // ===== MÉTODO PARA RECIBIR DAÑO =====
    public void RecibirDanio(int cantidad = 1)
    {
        // Evitar recibir daño si está muriendo
        if (estaMuriendo) return;

        // Restar vidas
        vidasActuales -= cantidad;

        // Actualizar UI de corazones
        if (healthUI != null)
        {
            healthUI.QuitarVida(cantidad);
        }

        // Verificar si el jugador murió
        if (vidasActuales <= 0)
        {
            Morir();
        }
        else
        {
            // Reproducir animación de recibir daño
            RecibirDanioAnimacion();
        }
    }

    // ===== ANIMACIÓN DE RECIBIR DAÑO =====
    private void RecibirDanioAnimacion()
    {
        // Evitar superposición de animaciones de daño
        if (estaRecibiendo) return;

        estaRecibiendo = true;

        // Activar trigger de recibir daño en el Animator
        if (animator != null)
        {
            animator.SetTrigger("Daño"); // Asegúrate de tener este trigger en tu Animator
        }

        // Resetear el flag después de un tiempo (duración de la animación)
        Invoke(nameof(FinAnimacionDanio), 0.5f); // Ajusta según la duración de tu animación
    }

    // ===== FIN DE ANIMACIÓN DE DAÑO =====
    private void FinAnimacionDanio()
    {
        estaRecibiendo = false;
    }

    // ===== MÉTODO DE MUERTE =====
    private void Morir()
    {
        // Evitar múltiples llamadas
        if (estaMuriendo) return;

        estaMuriendo = true;

        // Detener movimiento
        rb2D.velocity = Vector2.zero;

        // Desactivar el collider para que no reciba más daño
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Activar animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // NUEVO: Llamar a Game Over después de la animación
        Invoke(nameof(IrAGameOver), 1.5f);
    }

    // NUEVO: Método para ir a Game Over
    private void IrAGameOver()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    // ===== MÉTODO PARA DESTRUIR BALA (mantenido por compatibilidad) =====
    public void destroyBala()
    {
        Destroy(gameObject);
    }

    // ===== GETTERS PÚBLICOS =====
    public int GetVidasActuales() => vidasActuales;
    public int GetVidasMaximas() => vidasMaximas;
}