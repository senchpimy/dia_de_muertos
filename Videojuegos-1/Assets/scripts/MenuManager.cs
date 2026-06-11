using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static bool esVictoria = false;
    public static bool esGameOver = false;
    public TMP_Text textoTitulo; 

    void Start()
    {
        // Aseguramos que el cursor sea visible y esté libre al entrar al menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Cargamos la fuente temática (loco)
        TMP_FontAsset fuenteTematica = Resources.Load<TMP_FontAsset>("fonts/loco") ?? Resources.Load<TMP_FontAsset>("loco");

        // Si no se asignó en el inspector, buscamos el objeto del título
        if (textoTitulo == null)
        {
            // Buscamos el título ignorando el nombre antiguo
            GameObject objTitulo = GameObject.Find("Title") ?? GameObject.Find("Titulo") ?? GameObject.Find("YUKATAKIS");
            if (objTitulo != null) textoTitulo = objTitulo.GetComponent<TMP_Text>();
        }

        if (textoTitulo != null)
        {
            // Título temático y ajuste de tamaño
            textoTitulo.text = "OFRENDA: EL CAMINO DEL PAN";
            if (fuenteTematica != null) textoTitulo.font = fuenteTematica;
            textoTitulo.fontSize = 60; // Reducimos el tamaño
            textoTitulo.color = new Color(1.0f, 0.5f, 0.0f); // Naranja vibrante
            textoTitulo.alignment = TextAlignmentOptions.Center;

            if (esVictoria)
            {
                textoTitulo.text = "¡OFRENDA COMPLETADA!";
                textoTitulo.color = Color.green;
                esVictoria = false; 
            }
            else if (esGameOver)
            {
                textoTitulo.text = "ALMA PERDIDA";
                textoTitulo.color = Color.red;
                esGameOver = false;
            }
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
