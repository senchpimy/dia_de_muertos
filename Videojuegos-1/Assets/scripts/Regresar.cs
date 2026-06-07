using UnityEngine;
using UnityEngine.SceneManagement;

public class Regresar : MonoBehaviour
{
    public void RegresarMenu()
    {
        // Esto te devolverá a la pantalla de inicio
        SceneManager.LoadScene("MenuMain");
    }
}
