using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeVista : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void botonMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void botonFase() {
        SceneManager.LoadScene("Fase1");
    }
}
