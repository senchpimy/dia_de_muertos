using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VidaJugador : MonoBehaviour
{
    [Header("Salud del Jugador")]
    public int vida = 10;

    [Header("Interfaz (UI)")]
    public Slider barraDeVidaUI;

    void Start()
    {
        if (barraDeVidaUI != null)
        {
            barraDeVidaUI.maxValue = vida;
            barraDeVidaUI.value = vida;
        }
    }

    public void RecibirDanio(int danio)
    {
        vida -= danio;
        Debug.Log("Vida restante: " + vida);

        if (barraDeVidaUI != null)
        {
            barraDeVidaUI.value = vida;
        }

        if (vida <= 0)
        {
            Debug.Log("�Has muerto! Reiniciando nivel...");
            Morir();
        }
    }

    void Morir()
    {
        MenuManager.esGameOver = true;
        SceneManager.LoadScene("MenuMain");
    }
}
