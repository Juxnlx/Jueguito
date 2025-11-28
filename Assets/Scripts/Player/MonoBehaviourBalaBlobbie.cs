using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourBalaBlobbie : MonoBehaviour
{

    //velocidad de la bala
    public float speed;
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

}
