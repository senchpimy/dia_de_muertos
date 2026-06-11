using UnityEngine;

public class SistemaDisparo : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoDeVida = 3f;

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    void OnTriggerEnter(Collider otro)
    {
        VidaEnemigo enemigo = otro.GetComponent<VidaEnemigo>();

        if (enemigo != null)
        {
            enemigo.RecibirDanio(1);
        }

        Destroy(gameObject);
    }
}
