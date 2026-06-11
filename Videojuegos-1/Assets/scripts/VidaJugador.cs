using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar el nivel al morir
using UnityEngine.UI; // 1. OBLIGATORIO: Agregamos la librer�a de Interfaz de Usuario

public class VidaJugador : MonoBehaviour
{
    [Header("Salud del Jugador")]
    public int vida = 10;

    [Header("Interfaz (UI)")]
    public Slider barraDeVidaUI; // 2. Referencia para conectar nuestra barra visual

    // 3. Agregamos Start para configurar la barra al iniciar el nivel
    void Start()
    {
        if (barraDeVidaUI != null)
        {
            barraDeVidaUI.maxValue = vida; // Le decimos que el m�ximo es 10
            barraDeVidaUI.value = vida;    // La llenamos al m�ximo
        }
    }

    public void RecibirDanio(int danio)
    {
        vida -= danio;
        Debug.Log("�El esqueleto te golpe�! Vida restante: " + vida);

        // 4. Actualizamos el valor visual de la barra para que baje
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
        // En lugar de reiniciar, vamos al menú con pantalla de Game Over
        MenuManager.esGameOver = true;
        SceneManager.LoadScene("MenuMain");
    }
}