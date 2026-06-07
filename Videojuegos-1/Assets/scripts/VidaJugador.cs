using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar el nivel al morir
using UnityEngine.UI; // 1. OBLIGATORIO: Agregamos la librería de Interfaz de Usuario

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
            barraDeVidaUI.maxValue = vida; // Le decimos que el máximo es 10
            barraDeVidaUI.value = vida;    // La llenamos al máximo
        }
    }

    public void RecibirDanio(int danio)
    {
        vida -= danio;
        Debug.Log("¡El esqueleto te golpeó! Vida restante: " + vida);

        // 4. Actualizamos el valor visual de la barra para que baje
        if (barraDeVidaUI != null)
        {
            barraDeVidaUI.value = vida;
        }

        if (vida <= 0)
        {
            Debug.Log("¡Has muerto! Reiniciando nivel...");
            Morir();
        }
    }

    void Morir()
    {
        // Esto recarga la escena actual en la que estés jugando
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}