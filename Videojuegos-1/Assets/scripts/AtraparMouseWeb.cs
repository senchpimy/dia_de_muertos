using UnityEngine;
using UnityEngine.EventSystems; // ¡Súper importante para detectar la interfaz de usuario (botones)!

public class AtraparMouseWeb : MonoBehaviour
{
    void Update()
    {
        // 1. Si el jugador presiona ESC, liberamos el mouse para que pueda usar el menú de pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 2. Si el jugador hace clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            // LA MAGIA: Verificamos si el mouse está posado sobre un elemento de la UI (como tus botones)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // Si estamos tocando un botón, abortamos el bloqueo. ¡Te dejamos hacer clic!
                return;
            }

            // Si hicimos clic en el espacio vacío del juego (no en la UI), entonces sí lo bloqueamos
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}