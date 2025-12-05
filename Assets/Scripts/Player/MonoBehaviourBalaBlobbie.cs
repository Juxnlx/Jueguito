using UnityEngine;

public class MonoBehaviourBalaBlobbie : MonoBehaviour
{
    //velocidad de la bala
    public float speed = 10f;
    //almacena el rigidbody que recoge de unity
    private Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        //carga el ridigidbody
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        // Cálculo de la velocidad de la bala
        rb2D.velocity = Vector2.right * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si choca con algo que tenga "Base" en el nombre
        if (collision.gameObject.name.Contains("Base 1") ||
            collision.gameObject.name.Contains("Base 2") ||
            collision.gameObject.name.Contains("Base 3"))
        {
            Destroy(gameObject); // Destruye la bala
            return;
        }

        // Si choca con un enemigo
        if (collision.gameObject.name.Contains("Enemy"))
        {
            // Aquí tu código de daño al enemigo
            Destroy(gameObject);
        }
    }

}
