using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Método para el botón "Jugar"
    public void Jugar()
    {
        // Carga la escena principal del juego. 
        // Si prefieres que "Jugar" mande primero al tutorial, cambia "Nivel 1" por "Tutorial".
        SceneManager.LoadScene("Tutorial");
    }

    // Método para el botón "Selector de nivel"
    public void SelectorDeNivel()
    {
        // Carga exactamente el nombre de tu nueva escena
        SceneManager.LoadScene("SelectorNivel");
    }

    // Método para el botón "Créditos"
    public void AbrirCreditos()
    {
        // Carga exactamente la escena que tienes llamada "Creditos"
        SceneManager.LoadScene("Creditos");
    }

    
}