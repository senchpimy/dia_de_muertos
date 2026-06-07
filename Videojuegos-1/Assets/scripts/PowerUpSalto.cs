using UnityEngine;
using System.Collections; // Necesario para los tiempos de espera

public class PowerUpSalto : MonoBehaviour
{
    public float multiplicadorSalto = 2.0f; // Duplica el salto
    public float duracion = 5.0f;           // 5 segundos de poder

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos el script de movimiento en el jugador
            // (Asegúrate de que el script de movimiento de tu robot se llame así, 
            // si tiene otro nombre, cámbialo aquí)
            var movimiento = other.GetComponent<StarterAssets.ThirdPersonController>();

            if (movimiento != null)
            {
                StartCoroutine(AplicarPoder(movimiento));
            }
        }
    }

    IEnumerator AplicarPoder(StarterAssets.ThirdPersonController mov)
    {
        // 1. Aplicamos el poder
        float saltoOriginal = mov.JumpHeight;
        mov.JumpHeight *= multiplicadorSalto;

        // 2. Ocultamos el ítem (para que no lo recojan dos veces)
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // 3. Esperamos el tiempo del efecto
        yield return new WaitForSeconds(duracion);

        // 4. Regresamos a la normalidad
        mov.JumpHeight = saltoOriginal;
        Destroy(gameObject);
    }
}