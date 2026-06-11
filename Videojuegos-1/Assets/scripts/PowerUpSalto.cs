using UnityEngine;
using System.Collections;

public class PowerUpSalto : MonoBehaviour
{
    public float multiplicadorSalto = 2.0f;
    public float duracion = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var movimiento = other.GetComponent<StarterAssets.ThirdPersonController>();

            if (movimiento != null)
            {
                StartCoroutine(AplicarPoder(movimiento));
            }
        }
    }

    IEnumerator AplicarPoder(StarterAssets.ThirdPersonController mov)
    {
        float saltoOriginal = mov.JumpHeight;
        mov.JumpHeight *= multiplicadorSalto;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(duracion);

        mov.JumpHeight = saltoOriginal;
        Destroy(gameObject);
    }
}
