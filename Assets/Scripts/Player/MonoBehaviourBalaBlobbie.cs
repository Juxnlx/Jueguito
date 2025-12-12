using UnityEngine;

public class MonoBehaviourBalaBlobbie : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb2D;
    private bool yaImpacto = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb2D.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        if (!yaImpacto)
        {
            rb2D.velocity = Vector2.right * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (yaImpacto) return;

        // Si impacta con una base, destruir la bala
        if (other.GetComponent<Base>() != null)
        {
            yaImpacto = true;
            // La base NO recibe daño de las balas del jugador
            Destroy(gameObject);
            return;
        }

        // Si contiene "Base" en el nombre (tu código original)
        if (other.gameObject.name.Contains("Base"))
        {
            yaImpacto = true;
            Destroy(gameObject);
            return;
        }

        // Si impacta con un enemigo
        Enemigo enemigo = other.GetComponent<Enemigo>();
        if (enemigo != null)
        {
            yaImpacto = true;
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}