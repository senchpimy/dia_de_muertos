using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // M�todo para el bot�n "Jugar"
    public void Jugar()
    {
        // Carga la escena principal del juego. 
        SceneManager.LoadScene("Nivel 1");
    }

    // M�todo para el bot�n "Selector de nivel"
    public void SelectorDeNivel()
    {
        // Carga exactamente el nombre de tu nueva escena
        SceneManager.LoadScene("SelectorNivel");
    }

    // Mtodo para el botn "Crditos"
    public void AbrirCreditos()
    {
        // Carga exactamente la escena que tienes llamada "Creditos"
        SceneManager.LoadScene("Creditos");
    }

    public void IrAEscenarioCalavera()
    {
        SceneManager.LoadScene("EscenarioCalavera");
    }
    }