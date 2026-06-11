using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonTeletransporte : MonoBehaviour
{
    public string nombreEscenaDestino = "EscenarioCalavera";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            Debug.Log("Boton presionado: " + nombreEscenaDestino);
            SceneManager.LoadScene(nombreEscenaDestino);
        }
    }
}
