using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class MetaTutorial : MonoBehaviour
{
    // Cuando algo entra en la placa...
    private void OnTriggerEnter(Collider otro)
    {
        // Revisamos si el objeto que pisó la placa es el jugador
        if (otro.CompareTag("Player"))
        {
            Debug.Log("¡Tutorial Completado! Viajando al Nivel 1...");

            // Pon aquí el nombre EXACTO de tu escena del Nivel 1
            SceneManager.LoadScene("Nivel 1");
        }
    }
}