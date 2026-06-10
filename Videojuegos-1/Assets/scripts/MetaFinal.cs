using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MetaFinal : MonoBehaviour
{
    public Color colorVictoria = Color.green;
    private bool juegoTerminado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (juegoTerminado) return;

        // Verificamos si lo que entró es un Pan de Muerto (tiene la etiqueta Cargable)
        // Y verificamos que NO esté siendo cargado (no tiene padre)
        if (other.CompareTag("Cargable") && other.transform.parent == null)
        {
            StartCoroutine(ProcesoDeVictoria());
        }
    }

    IEnumerator ProcesoDeVictoria()
    {
        juegoTerminado = true;
        Debug.Log("¡FELICIDADES! Has entregado el Pan de Muerto.");
        
        // Cambiar color de la plataforma y luz
        GetComponent<Renderer>().material.color = colorVictoria;
        Light luz = GetComponentInChildren<Light>();
        if (luz != null) luz.color = colorVictoria;

        // Esperar 2 segundos para que el jugador vea que lo logró
        yield return new WaitForSeconds(2f);

        // Activar el modo victoria y cargar el menú principal
        MenuManager.esVictoria = true;
        SceneManager.LoadScene("MenuMain");
    }
}
