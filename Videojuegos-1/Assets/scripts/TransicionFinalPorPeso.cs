using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionFinalPorPeso : MonoBehaviour
{
    [Header("Configuración del Puzzle de Físicas")]
    public float pesoActual = 0f;
    public float pesoRequerido = 3f;

    [Header("Escena de Salida")]
    public string escenaDestino = "Creditos";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cargable") && other.attachedRigidbody != null)
        {
            pesoActual += other.attachedRigidbody.mass;
            Debug.Log("Peso actual en la meta: " + pesoActual);

            if (pesoActual >= pesoRequerido)
            {
                TerminarNivel();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cargable") && other.attachedRigidbody != null)
        {
            pesoActual -= other.attachedRigidbody.mass;

            if (pesoActual < 0f)
            {
                pesoActual = 0f;
            }

            Debug.Log("Peso actual en la meta: " + pesoActual);
        }
    }

    private void TerminarNivel()
    {
        Debug.Log("¡Nivel superado! Cargando pantalla de créditos...");
        SceneManager.LoadScene(escenaDestino);
    }
}
