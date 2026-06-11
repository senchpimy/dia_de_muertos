using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeNivel : MonoBehaviour
{
    [Header("Configuración del Nivel")]
    public string nombreSiguienteNivel = "Nivel 1";

    void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Player"))
        {
            Debug.Log("Viajando al " + nombreSiguienteNivel);
            SceneManager.LoadScene(nombreSiguienteNivel);
        }
    }
}
