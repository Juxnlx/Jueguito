using UnityEngine;

public class BalaEnemigo : MonoBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.velocity = Vector2.left * speed; // se mueve hacia la izquierda
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con el jugador
        if (other.CompareTag("Player"))
        {
            // Aquí puedes restar vida al jugador
            Destroy(gameObject); // destruir bala
        }

        // Si choca con algo más (opcional)
        if (other.CompareTag("Obstaculo"))
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Destruir bala después de 3 segundos para no saturar la escena
        Destroy(gameObject, 3f);
    }
}
