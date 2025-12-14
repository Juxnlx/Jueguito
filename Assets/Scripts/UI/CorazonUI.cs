using UnityEngine;
using UnityEngine.UI;

// Cómo crear un sistema de corazones en Unity (Vida como corazones)

public class CorazonUI : MonoBehaviour
{
    [SerializeField] private Image imagenCorazon;
    [SerializeField] private bool estaActivo;

    public void ActivarCorazon()
    {
        imagenCorazon.enabled = true;
        estaActivo = true;
    }

    public void DesactivarCorazon()
    {
        imagenCorazon.enabled = false;
        estaActivo = false;
    }

    public bool EstaActivo() => estaActivo;
}