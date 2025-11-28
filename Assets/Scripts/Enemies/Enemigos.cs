using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigos : MonoBehaviour
{
    public Enemigo[] enemigosPrefabs; // Prefabs de enemigos, uno por fila

    public int filas = 5;
    public int columnas = 6;

    public float separacionX = 4.0f; // Separación horizontal
    public float separacionY = 4.0f; // Separación vertical
    public float velocidad = 2.0f;   // Velocidad de movimiento

    private bool subiendo = false;    // Dirección vertical: true = arriba, false = abajo
    private float limiteSuperior;     // Y máximo de la cámara
    private float limiteInferior;     // Y mínimo de la cámara

    private void Awake()
    {
        // Crear la formación de enemigos
        Vector3 startPos = transform.position;

        for (int fila = 0; fila < filas; fila++)
        {
            Vector3 posicionFila = new Vector3(
                startPos.x,
                startPos.y - fila * separacionY,
                0
            );

            for (int col = 0; col < columnas; col++)
            {
                Enemigo enemigo = Instantiate(
                    enemigosPrefabs[fila % enemigosPrefabs.Length],
                    this.transform
                );

                Vector3 posicion = posicionFila;
                posicion.x += col * separacionX;
                enemigo.transform.position = posicion;
            }
        }

        // Calcular límites de la cámara en Y
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        limiteInferior = bottomLeft.y;
        limiteSuperior = topRight.y;
    }

    private void Update()
    {
        // Determinar dirección vertical
        float dirY = subiendo ? 1 : -1;

        // Mover toda la formación verticalmente
        transform.position += Vector3.up * dirY * velocidad * Time.deltaTime;

        // Revisar si algún enemigo tocó los límites superior o inferior
        foreach (Transform enemigo in transform)
        {
            if (!enemigo.gameObject.activeInHierarchy)
                continue;

            if (!subiendo && enemigo.position.y <= limiteInferior)
            {
                CambiarDireccion();
                break;
            }
            else if (subiendo && enemigo.position.y >= limiteSuperior)
            {
                CambiarDireccion();
                break;
            }
        }
    }

    // Función para cambiar la dirección vertical y mover un paso hacia la izquierda
    private void CambiarDireccion()
    {
        // Cambiamos la dirección vertical
        subiendo = !subiendo;

        // Retrocedemos un paso hacia la izquierda
        Vector3 pos = transform.position;
        pos.x -= separacionX; // Paso hacia la izquierda
        transform.position = pos;
    }
}



