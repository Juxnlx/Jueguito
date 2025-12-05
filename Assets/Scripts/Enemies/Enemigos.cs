using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigos : MonoBehaviour
{
    public Enemigo prefabM; // Minotauro
    public Enemigo prefabS; // Seta

    public int filas = 4;
    public int columnas = 5;

    public float separacionX = 0.1f;
    public float separacionY = 0.35f;
    public float velocidad = 0.7f;

    public float pasoIzquierda = 0.2f;

    private bool subiendo = false;
    private float limiteSuperior;
    private float limiteInferior;

    private void Awake()
    {
        // Ancho y alto máximo según prefabs
        float anchoMax = Mathf.Max(prefabM.GetComponent<SpriteRenderer>().bounds.size.x,
                                   prefabS.GetComponent<SpriteRenderer>().bounds.size.x);
        float altoMax = Mathf.Max(prefabM.GetComponent<SpriteRenderer>().bounds.size.y,
                                  prefabS.GetComponent<SpriteRenderer>().bounds.size.y);

        float margen = 0.05f;
        separacionX = anchoMax + margen;
        separacionY = altoMax + margen;

        Vector3 startPos = new Vector3(
            -separacionX * (columnas - 1) / 2f,
            transform.position.y,
            0
        );

        int numColumnasS = 3; // primeras 3 columnas S
        for (int fila = 0; fila < filas; fila++)
        {
            Vector3 posicionFila = new Vector3(
                startPos.x,
                startPos.y - fila * separacionY,
                0
            );

            for (int col = 0; col < columnas; col++)
            {
                Enemigo prefabElegido = (col < numColumnasS) ? prefabS : prefabM;
                Enemigo enemigo = Instantiate(prefabElegido, this.transform);

                Vector3 posicion = posicionFila;
                posicion.x += col * separacionX;
                enemigo.transform.position = posicion;
            }
        }

        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        limiteInferior = bottomLeft.y;
        limiteSuperior = topRight.y;
    }

    private void Update()
    {
        float dirY = subiendo ? 1 : -1;
        transform.position += Vector3.up * dirY * velocidad * Time.deltaTime;

        foreach (Transform enemigo in transform)
        {
            if (!enemigo.gameObject.activeInHierarchy) continue;

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

    private void CambiarDireccion()
    {
        subiendo = !subiendo;
        Vector3 pos = transform.position;
        pos.x -= pasoIzquierda;
        transform.position = pos;
    }
}