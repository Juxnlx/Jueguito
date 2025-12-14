using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Tooltip("Arrastra aquí la escena desde el Project")]
    public Object escena;

    public void CambiarEscena()
    {
        if (escena != null)
        {
            SceneManager.LoadScene(escena.name);
        }
        else
        {
            Debug.LogError("No se ha asignado ninguna escena!");
        }
    }
}