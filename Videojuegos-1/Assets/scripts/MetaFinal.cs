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

        if (other.CompareTag("Cargable") && other.transform.parent == null)
        {
            StartCoroutine(ProcesoDeVictoria());
        }
    }

    IEnumerator ProcesoDeVictoria()
    {
        juegoTerminado = true;
        Debug.Log("¡FELICIDADES! Has entregado el Pan de Muerto.");
        
        GetComponent<Renderer>().material.color = colorVictoria;
        Light luz = GetComponentInChildren<Light>();
        if (luz != null) luz.color = colorVictoria;

        yield return new WaitForSeconds(2f);

        MenuManager.esVictoria = true;
        SceneManager.LoadScene("MenuMain");
    }
}
