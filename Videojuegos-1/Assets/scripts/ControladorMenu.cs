using UnityEngine;
using UnityEngine.SceneManagement; // OBLIGATORIO para cambiar de escenas

public class ControladorMenu : MonoBehaviour
{
    // Esta es la que ya tenías para el botón de Salir
    public void SalirDelJuego()
    {
        Debug.Log("Cerrando el juego...");
        Application.Quit();
    }

    // NUEVA FUNCIÓN: Para viajar a otra escena
    public void CambiarEscena(string nombreDeLaEscena)
    {
        SceneManager.LoadScene(nombreDeLaEscena);
    }
}