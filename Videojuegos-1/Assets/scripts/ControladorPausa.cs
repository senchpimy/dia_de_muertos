using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ControladorPausa : MonoBehaviour
{
    public GameObject panelPausa;
    private bool juegoPausado = false;

    void Start()
    {
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    void Update()
    {
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
        panelPausa.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RegresarAlMenuPrincipal()
    {
        Time.timeScale = 1f; 

        SceneManager.LoadScene("MenuMain");
    }
}
