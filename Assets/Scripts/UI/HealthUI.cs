using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [Header("Configuración de Corazones")]
    public GameObject heartPrefab;
    public Transform heartsContainer;
    public int maxVidas = 3;

    [Header("Sprites de Corazón")]
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartEmpty;

    [Header("Animaciones (Arrastra aquí los Animation Clips)")]
    [SerializeField] private AnimationClip animacionPerderVida;
    [SerializeField] private AnimationClip animacionGanarVida;
    [SerializeField] private AnimationClip animacionIdle;

    [Header("Configuración de Reproducción")]
    [SerializeField] private bool usarAnimaciones = true;
    [SerializeField] private bool esperarFinAnimacion = true;

    private List<Image> hearts = new List<Image>();
    private List<Animation> heartAnimations = new List<Animation>();
    private int vidasActuales;

    void Start()
    {
        vidasActuales = maxVidas;
        CrearCorazones();
        ActualizarCorazones();
    }

    void CrearCorazones()
    {
        // Limpiar corazones existentes
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }
        hearts.Clear();
        heartAnimations.Clear();

        // Crear nuevos corazones
        for (int i = 0; i < maxVidas; i++)
        {
            GameObject nuevoCorazon = Instantiate(heartPrefab, heartsContainer);
            Image imagenCorazon = nuevoCorazon.GetComponent<Image>();

            // Añadir componente Animation si no existe
            Animation animComp = nuevoCorazon.GetComponent<Animation>();
            if (animComp == null && usarAnimaciones)
            {
                animComp = nuevoCorazon.AddComponent<Animation>();
            }

            // Añadir los clips de animación
            if (animComp != null && usarAnimaciones)
            {
                if (animacionPerderVida != null)
                {
                    animComp.AddClip(animacionPerderVida, "PerderVida");
                }
                if (animacionGanarVida != null)
                {
                    animComp.AddClip(animacionGanarVida, "GanarVida");
                }
                if (animacionIdle != null)
                {
                    animComp.AddClip(animacionIdle, "Idle");
                    animComp.clip = animacionIdle;
                }
            }

            hearts.Add(imagenCorazon);
            heartAnimations.Add(animComp);
        }
    }

    void ActualizarCorazones()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < vidasActuales)
            {
                hearts[i].sprite = heartFull;
                hearts[i].enabled = true;
            }
            else
            {
                if (heartEmpty != null)
                {
                    hearts[i].sprite = heartEmpty;
                    hearts[i].enabled = true;
                }
                else
                {
                    hearts[i].enabled = false;
                }
            }
        }
    }

    // Quitar vida con animación
    public void QuitarVida(int cantidad = 1)
    {
        StartCoroutine(QuitarVidaConAnimacion(cantidad));
    }

    private IEnumerator QuitarVidaConAnimacion(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            if (vidasActuales > 0)
            {
                vidasActuales--;

                // Reproducir animación de perder vida
                if (usarAnimaciones && heartAnimations[vidasActuales] != null && animacionPerderVida != null)
                {
                    Animation anim = heartAnimations[vidasActuales];
                    anim.Play("PerderVida");

                    if (esperarFinAnimacion)
                    {
                        // Esperar a que termine la animación
                        yield return new WaitForSeconds(animacionPerderVida.length);
                    }
                    else
                    {
                        // Esperar un frame
                        yield return null;
                    }
                }

                // Cambiar sprite después de la animación
                if (heartEmpty != null)
                {
                    hearts[vidasActuales].sprite = heartEmpty;
                }
                else
                {
                    hearts[vidasActuales].enabled = false;
                }

                // Reproducir animación idle si existe
                if (usarAnimaciones && heartAnimations[vidasActuales] != null && animacionIdle != null)
                {
                    heartAnimations[vidasActuales].Play("Idle");
                }
            }
        }

        if (vidasActuales <= 0)
        {
            Debug.Log("¡Game Over!");
            // Aquí puedes llamar a tu lógica de Game Over
        }
    }

    // Añadir vida con animación
    public void AñadirVida(int cantidad = 1)
    {
        StartCoroutine(AñadirVidaConAnimacion(cantidad));
    }

    private IEnumerator AñadirVidaConAnimacion(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            if (vidasActuales < maxVidas)
            {
                // Cambiar sprite primero
                hearts[vidasActuales].sprite = heartFull;
                hearts[vidasActuales].enabled = true;

                // Reproducir animación de ganar vida
                if (usarAnimaciones && heartAnimations[vidasActuales] != null && animacionGanarVida != null)
                {
                    Animation anim = heartAnimations[vidasActuales];
                    anim.Play("GanarVida");

                    if (esperarFinAnimacion)
                    {
                        yield return new WaitForSeconds(animacionGanarVida.length);
                    }
                    else
                    {
                        yield return null;
                    }
                }

                vidasActuales++;

                // Reproducir animación idle si existe
                if (usarAnimaciones && heartAnimations[vidasActuales - 1] != null && animacionIdle != null)
                {
                    heartAnimations[vidasActuales - 1].Play("Idle");
                }
            }
        }
    }

    // Método para reproducir animación manualmente en un corazón específico
    public void ReproducirAnimacion(int indiceCorazon, string nombreAnimacion)
    {
        if (indiceCorazon >= 0 && indiceCorazon < heartAnimations.Count)
        {
            if (heartAnimations[indiceCorazon] != null)
            {
                heartAnimations[indiceCorazon].Play(nombreAnimacion);
            }
        }
    }

    // Métodos sin animación (instantáneos)
    public void EstablecerVidas(int nuevasVidas)
    {
        vidasActuales = Mathf.Clamp(nuevasVidas, 0, maxVidas);
        ActualizarCorazones();
    }

    public void CambiarVidasMaximas(int nuevoMax)
    {
        maxVidas = nuevoMax;
        vidasActuales = Mathf.Min(vidasActuales, maxVidas);
        CrearCorazones();
        ActualizarCorazones();
    }

    // Getters
    public int GetVidasActuales() => vidasActuales;
    public int GetVidasMaximas() => maxVidas;

    // Ejemplo de uso
    void Update()
    {
        // Presiona Q para quitar vida, E para añadir vida
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitarVida();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AñadirVida();
        }
    }
}