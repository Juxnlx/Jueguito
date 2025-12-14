using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    public Action<int> JugadorPerderVida;
    //public Action<int> JugadorSeCuro;

    [SerializeField] private int vidaMaxima =3;
    [SerializeField] private int vidaActual;

    private void Awake()
    {
        vidaActual = vidaMaxima;
    }

    public void perderVida()
    {
        int vidaTemporal = vidaActual - 1;

        vidaTemporal = Mathf.Clamp(vidaTemporal, 0, vidaMaxima);

        vidaActual = vidaTemporal;

        JugadorPerderVida?.Invoke(vidaActual);

        if (vidaActual <= 0)
        {
            Perder();
        }
    }

    private void Perder()
    {
        //Cambiamos de escena o mostramos pantalla de game over
        SceneManager.LoadScene("Asserts/Scenes/GameOver");
    }

    /*
    public void CurarVida(int curacion)
    {
        int vidaTemporal = vidaActual + curacion;

        vidaTemporal = Mathf.Clamp(vidaTemporal, 0, vidaMaxima);

        vidaActual = vidaTemporal;

        JugadorSeCuro?.Invoke(vidaActual);
    }
    */


    public int GetVidaMaxima() => vidaMaxima;
    public int GetVidaActual() => vidaActual;
}