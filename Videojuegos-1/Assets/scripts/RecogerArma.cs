using UnityEngine;

public class RecogerArma : MonoBehaviour
{
    [Header("El arma oculta del jugador")]
    public GameObject armaEnMano;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (armaEnMano != null)
            {
                armaEnMano.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
