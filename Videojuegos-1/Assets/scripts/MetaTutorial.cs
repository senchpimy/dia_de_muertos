using UnityEngine;
using UnityEngine.SceneManagement;

public class MetaTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Player"))
        {
            Debug.Log("¡Tutorial Completado! Viajando al Nivel 1...");

            SceneManager.LoadScene("Nivel 1");
        }
    }
}
