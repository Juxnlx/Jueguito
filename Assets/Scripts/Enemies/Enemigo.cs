using UnityEngine;

public class Enemigo : MonoBehaviour
{
    // Array de sprites que se usan para la animación del enemigo
    public Sprite[] spritesAnimacion;
    public float tiempoAnimacion = 1f;

    // SpriteRenderer que se encarga de mostrar el sprite en pantalla
    private SpriteRenderer renderizadorSprite;
    private int indiceAnimacion;

    // Columna a la que pertenece este enemigo
    public int columna;

    private void Awake()
    {
        renderizadorSprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimarSprite), tiempoAnimacion, tiempoAnimacion);
    }

    private void AnimarSprite()
    {
        indiceAnimacion++;
        if (indiceAnimacion >= spritesAnimacion.Length)
            indiceAnimacion = 0;

        renderizadorSprite.sprite = spritesAnimacion[indiceAnimacion];
    }

    private void OnDestroy()
    {
        // Actualiza los enemigos frontales en el gestor
        Enemigos gestor = GetComponentInParent<Enemigos>();
        if (gestor != null)
        {
            gestor.ActualizarEnemigosFrontales();
        }
    }
}
