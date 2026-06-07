using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena
using UnityEngine.InputSystem; // Necesario para el teclado moderno

public class ControladorPausa : MonoBehaviour
{
    public GameObject panelPausa; // El Panel oscuro que contiene los botones
    private bool juegoPausado = false;

    void Start()
    {
        // Al empezar el nivel, nos aseguramos de que la pausa esté apagada
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    void Update()
    {
        // Detectamos si el jugador presiona la tecla Escape
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (juegoPausado)
            {
                ReanudarJuego();
            }
            else
            {
                PausarJuego();
            }
        }
    }

    public void PausarJuego()
    {
        juegoPausado = true;
        panelPausa.SetActive(true); // Mostramos el menú
        Time.timeScale = 0f;        // Congelamos el tiempo del juego (físicas y movimientos)

        // Liberamos el cursor del ratón para poder hacer clic
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        panelPausa.SetActive(false); // Ocultamos el menú
        Time.timeScale = 1f;         // Devolvemos el tiempo a la normalidad

        // Volvemos a ocultar y bloquear el cursor para seguir jugando
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RegresarAlMenuPrincipal()
    {
        Time.timeScale = 1f; // ¡Súper importante! Devolver el tiempo a la normalidad antes de salir

        // Reemplaza "MenuPrincipal" por el nombre exacto de tu escena del menú
        SceneManager.LoadScene("MenuMain");
    }
}