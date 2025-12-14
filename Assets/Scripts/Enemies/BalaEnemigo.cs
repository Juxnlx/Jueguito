using UnityEngine;

public class BalaEnemigo : MonoBehaviour
{
    // ===== CONFIGURACIÓN DE MOVIMIENTO =====
    public float speed = 10f; // Velocidad de la bala
    private Rigidbody2D rb2D; // Componente de física

    // ===== CONTROL DE IMPACTO =====
    private bool yaImpacto = false; // Flag para evitar múltiples impactos

    void Start()
    {
        // Inicializar componente de física
        rb2D = GetComponent<Rigidbody2D>();

        // Configurar física para mejor detección de colisiones
        rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb2D.gravityScale = 0; // Sin gravedad

        // Aplicar velocidad hacia la izquierda (hacia el jugador)
        rb2D.velocity = Vector2.left * speed;

        // Destruir automáticamente después de 3 segundos (por si sale de pantalla)
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si ya impactó, no procesar más colisiones
        if (yaImpacto) return;

        // ===== IMPACTO CON EL JUGADOR =====
        if (other.CompareTag("Player"))
        {
            yaImpacto = true;

            // Intentar hacer daño al jugador
            MonoBehaviourBlobbie jugador = other.GetComponent<MonoBehaviourBlobbie>();
            if (jugador != null)
            {
                jugador.RecibirDanio(1); // Hacer 1 punto de daño
            }

            // Destruir la bala
            Destroy(gameObject);
            return;
        }

        // ===== IMPACTO CON UNA BASE =====
        Base baseObj = other.GetComponent<Base>();
        if (baseObj != null)
        {
            yaImpacto = true;
            baseObj.RecibirDanio(1); // Hacer daño a la base
            Destroy(gameObject);
            return;
        }

        // ===== IMPACTO CON UN OBSTÁCULO =====
        if (other.CompareTag("Obstaculo"))
        {
            yaImpacto = true;
            Destroy(gameObject);
        }
    }
}