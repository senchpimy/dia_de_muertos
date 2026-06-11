using UnityEngine;
using System.Collections.Generic;

public class BotonDePeso : MonoBehaviour
{
    public float pesoMinimo = 3.0f;
    public GameObject puerta;

    private List<Rigidbody> objetosEnPlaca = new List<Rigidbody>();
    private float pesoActualEnPlaca = 0f;

    private void OnTriggerEnter(Collider otro)
    {
        Rigidbody rb = otro.GetComponent<Rigidbody>();
        if (rb != null && !objetosEnPlaca.Contains(rb))
        {
            objetosEnPlaca.Add(rb);
        }
    }

    private void OnTriggerExit(Collider otro)
    {
        Rigidbody rb = otro.GetComponent<Rigidbody>();
        if (rb != null && objetosEnPlaca.Contains(rb))
        {
            objetosEnPlaca.Remove(rb);
        }
    }

    private void Update()
    {
        float pesoCalculado = 0f;

        objetosEnPlaca.RemoveAll(item => item == null);

        foreach (Rigidbody rb in objetosEnPlaca)
        {
            if (!rb.isKinematic)
            {
                pesoCalculado += rb.mass;
            }
        }

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
            Debug.Log("Boton ACTIVADO" + pesoActualEnPlaca);
            transform.position = new Vector3(transform.position.x, 0.05f, transform.position.z);
            if (puerta != null) puerta.SetActive(false);
        }
        else
        {
            Debug.Log("Boton DESACTIVADO" + pesoActualEnPlaca);
            transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
            if (puerta != null) puerta.SetActive(true);
        }
    }
}
