using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonTeletransporte : MonoBehaviour
{
    public string nombreEscenaDestino = "EscenarioCalavera";

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos si es el jugador quien toca el botón
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            Debug.Log("¡Botón presionado! Viajando a: " + nombreEscenaDestino);
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}
