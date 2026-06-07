using UnityEngine;
using System.Collections.Generic; // Necesario para hacer listas

public class BotonDePeso : MonoBehaviour
{
    public float pesoMinimo = 3.0f;
    public GameObject puerta;

    // La placa llevará una lista de los objetos que están en su zona
    private List<Rigidbody> objetosEnPlaca = new List<Rigidbody>();
    private float pesoActualEnPlaca = 0f;

    private void OnTriggerEnter(Collider otro)
    {
        Rigidbody rb = otro.GetComponent<Rigidbody>();
        // Si entra un objeto con físicas y no está en la lista, lo agregamos
        if (rb != null && !objetosEnPlaca.Contains(rb))
        {
            objetosEnPlaca.Add(rb);
        }
    }

    private void OnTriggerExit(Collider otro)
    {
        Rigidbody rb = otro.GetComponent<Rigidbody>();
        // Si el objeto se va, lo quitamos de la lista
        if (rb != null && objetosEnPlaca.Contains(rb))
        {
            objetosEnPlaca.Remove(rb);
        }
    }

    // Update se ejecuta todo el tiempo, revisando matemáticamente la placa
    private void Update()
    {
        float pesoCalculado = 0f;

        // Limpiamos la lista por si algún objeto fue destruido
        objetosEnPlaca.RemoveAll(item => item == null);

        // Revisamos uno por uno los objetos que están tocando la placa
        foreach (Rigidbody rb in objetosEnPlaca)
        {
            // EL TRUCO: Solo sumamos su peso si NO es Kinematic (es decir, si NO lo tienes en las manos)
            if (!rb.isKinematic)
            {
                pesoCalculado += rb.mass;
            }
        }

        // Solo si el peso cambió, evaluamos la puerta para no saturar la consola
        if (pesoCalculado != pesoActualEnPlaca)
        {
            pesoActualEnPlaca = pesoCalculado;
            EvaluarPuerta();
        }
    }

    private void EvaluarPuerta()
    {
        if (pesoActualEnPlaca >= pesoMinimo)
        {
            Debug.Log("✅ ¡Botón ACTIVADO! Peso válido: " + pesoActualEnPlaca);
            transform.position = new Vector3(transform.position.x, 0.05f, transform.position.z);
            if (puerta != null) puerta.SetActive(false);
        }
        else
        {
            Debug.Log("❌ Botón DESACTIVADO. Peso válido: " + pesoActualEnPlaca);
            transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
            if (puerta != null) puerta.SetActive(true);
        }
    }
}