using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourBlobbie : MonoBehaviour
{

    //atributo privado llamado vertical
    private float vertical;
    //atributo que controla la velocidad de Blobbie
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //almacena los valores de la y
        vertical = Input.GetAxisRaw("Vertical");
    }
}
