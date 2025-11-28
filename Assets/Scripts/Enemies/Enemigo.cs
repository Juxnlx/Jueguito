using UnityEngine;

public class Enemigo : MonoBehaviour
{
    // Array de sprites que se usan para la animación del enemigo
    public Sprite[] spritesAnimacion;

    // Tiempo (en segundos) entre cada frame de animación
    public float tiempoAnimacion = 0.2f;

    // SpriteRenderer que se encarga de mostrar el sprite en pantalla
    private SpriteRenderer renderizadorSprite;

    // Índice del frame actual de la animación
    private int indiceAnimacion;

    private void Awake()
    {
        // Obtenemos el componente SpriteRenderer del enemigo
        renderizadorSprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Llama a la función AnimarSprite por primera vez después de tiempoAnimacion
        // y luego se repite cada tiempoAnimacion segundos
        InvokeRepeating(nameof(AnimarSprite), tiempoAnimacion, tiempoAnimacion);
    }

    private void AnimarSprite()
    {
        // Avanzamos al siguiente frame de animación
        indiceAnimacion++;

        // Si llegamos al final del array de sprites, volvemos al primer frame
        if (indiceAnimacion >= spritesAnimacion.Length)
        {
            indiceAnimacion = 0;
        }

        // Asignamos el sprite actual al SpriteRenderer
        renderizadorSprite.sprite = spritesAnimacion[indiceAnimacion];
    }
}
