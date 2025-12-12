using UnityEngine;

public class Base : MonoBehaviour
{
    // Vida de la base
    public int vidaMaxima = 5;
    private int vidaActual;

    // Sprites para mostrar el daño progresivo (opcional)
    public Sprite[] spritesEstados; // 0=intacta, 1=poco daño, 2=medio daño, 3=muy dañada
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        vidaActual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Asegurarse de que el collider es un trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si le impacta una bala enemiga
        if (other.GetComponent<BalaEnemigo>() != null)
        {
            RecibirDanio();
            Destroy(other.gameObject); // Destruir la bala
        }

        // Si le impacta un enemigo directamente (opcional)
        if (other.GetComponent<Enemigo>() != null)
        {
            RecibirDanio();
        }
    }

    private void RecibirDanio()
    {
        vidaActual--;

        // Actualizar sprite según el daño (si tienes sprites)
        ActualizarSprite();

        // Si la vida llega a 0, destruir la base
        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ActualizarSprite()
    {
        // Si no tienes sprites de daño, puedes cambiar el color
        if (spritesEstados != null && spritesEstados.Length > 0)
        {
            // Calcular qué sprite mostrar según la vida restante
            float porcentajeVida = (float)vidaActual / vidaMaxima;
            int indiceSprite = Mathf.Clamp(
                Mathf.FloorToInt((1f - porcentajeVida) * spritesEstados.Length),
                0,
                spritesEstados.Length - 1
            );

            if (spriteRenderer != null && indiceSprite < spritesEstados.Length)
            {
                spriteRenderer.sprite = spritesEstados[indiceSprite];
            }
        }
        else
        {
            // Alternativa: cambiar el color para mostrar el daño
            if (spriteRenderer != null)
            {
                float porcentajeVida = (float)vidaActual / vidaMaxima;
                // Ir de verde a rojo según el daño
                Color colorDanio = Color.Lerp(Color.red, Color.green, porcentajeVida);
                spriteRenderer.color = colorDanio;
            }
        }
    }
}