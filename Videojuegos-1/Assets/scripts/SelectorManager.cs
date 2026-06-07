using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectorManager : MonoBehaviour
{
    public void CargarTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void CargarNivel1()
    {
        SceneManager.LoadScene("Nivel 1");
    }

    public void CargarNivel2()
    {
        SceneManager.LoadScene("Nivel 2");
    }

    public void RegresarMenu()
    {
        // Esto te devolverá a la pantalla de inicio
        SceneManager.LoadScene("MenuMain");
    }
}