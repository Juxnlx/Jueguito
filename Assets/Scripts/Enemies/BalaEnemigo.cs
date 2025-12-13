using UnityEngine;

public class BalaEnemigo : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb2D;
    private bool yaImpacto = false;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.velocity = Vector2.left * speed;
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (yaImpacto) return;

        // Si choca con el jugador
        if (other.CompareTag("Player"))
        {
            yaImpacto = true;
            Destroy(gameObject);
            return;
        }

        // Si choca con una base
        Base baseObj = other.GetComponent<Base>();
        if (baseObj != null)
        {
            yaImpacto = true;
            baseObj.RecibirDanio(1); // NUEVO: Hacer daño a la base
            Destroy(gameObject);
            return;
        }

        // Si choca con un obstáculo
        if (other.CompareTag("Obstaculo"))
        {
            yaImpacto = true;
            Destroy(gameObject);
        }
    }
}