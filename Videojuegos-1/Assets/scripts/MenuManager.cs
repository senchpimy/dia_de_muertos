using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static bool esVictoria = false;
    public TMP_Text textoTitulo; 

    void Start()
    {
        // Si no se asign en el inspector, intentamos buscar el ttulo por cdigo
        if (textoTitulo == null)
        {
            // Buscamos un objeto que se llame 'Title' o similar, o simplemente el primer TMP_Text
            GameObject objTitulo = GameObject.Find("Title") ?? GameObject.Find("Titulo") ?? GameObject.Find("YUKATAKIS");
            if (objTitulo != null) textoTitulo = objTitulo.GetComponent<TMP_Text>();
        }

        if (esVictoria && textoTitulo != null)
        {
            textoTitulo.text = "¡VICTORIA!";
            textoTitulo.color = Color.green;
            esVictoria = false; 
        }
    }

    public void Jugar()
    {
        SceneManager.LoadScene("Nivel 1");
    }

    public void SelectorDeNivel()
    {
        SceneManager.LoadScene("SelectorNivel");
    }

    public void AbrirCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void IrAEscenarioCalavera()
    {
        SceneManager.LoadScene("EscenarioCalavera");
    }
}
