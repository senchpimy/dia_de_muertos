using UnityEngine;

public class RecogerArma : MonoBehaviour
{
    [Header("El arma oculta del jugador")]
    public GameObject armaEnMano;

    private void OnTriggerEnter(Collider other)
    {
        // Revisamos si el que tocó el cubo tiene la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            // 1. Encendemos el arma que estaba apagada
            if (armaEnMano != null)
            {
                armaEnMano.SetActive(true);
            }

            // 2. Destruimos este cubo flotante del piso
            Destroy(gameObject);
        }
    }
}