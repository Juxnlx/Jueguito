using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MonoBehaviourBlobbie : MonoBehaviour
{

    //atributo privado llamado vertical
    private float vertical;
    //atributo que controla la velocidad de Blobbie
    [SerializeField] private float speed;
    private Rigidbody2D rb2D;
    //sirve para poder acceder al animator del personaje
    private Animator animator;
    //almacena el prefab
    public GameObject prefabBala;
    //gestiona el último disparo
    private float ultimoDisparo;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        speed = 5; // Ajusta la velocidad según necesites

        // agarra el componente que está en el inspector. carga el componente animator del objeto en la variable
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //almacena los valores de la y
        vertical = Input.GetAxisRaw("Vertical");
        //actualiza la variable de run del animator para controlar las idle
        animator.SetBool("Run", vertical != 0.0f);

        if (Input.GetKey(KeyCode.Space) && Time.time > ultimoDisparo + 0.25f)
        {
            Shoot();
            ultimoDisparo = Time.time;
        }

    }

    private void FixedUpdate()
    {
        // Movimiento solo en el eje Y (arriba y abajo)
        rb2D.velocity = new Vector2(0, vertical * speed);
    }

    //Método que sirve para poder disparar balas
    private void Shoot()
    {
        GameObject bala = Instantiate(
            prefabBala,
            //no hace falte que cambie la posición de la bala porque siempre irá a la derecha
            transform.position + Vector3.right * 0.1f,
            Quaternion.identity
            );

    }
       
    //método para poder destruir la bala
    public void destroyBala()
    {
        Destroy(gameObject);
    }

}
