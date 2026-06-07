using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeNivel : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    public string nombreSiguienteNivel = "Nivel 1";

    // Esta función se activa en el instante en que algo toca la placa
    void OnTriggerEnter(Collider otro)
    {
        // Revisamos estrictamente si el que pisó la placa es el jugador
        if (otro.CompareTag("Player"))
        {
            Debug.Log("¡Pisaste la placa! Viajando al " + nombreSiguienteNivel);
            SceneManager.LoadScene(nombreSiguienteNivel);
        }
    }
}