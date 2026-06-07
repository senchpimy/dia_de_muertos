using UnityEngine;

public class SistemaDisparo : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoDeVida = 3f; // Tiempo antes de que la bala se destruya sola

    void Start()
    {
        // Si fallas el tiro, la bala se destruye en 3 segundos para no gastar memoria
        Destroy(gameObject, tiempoDeVida);
    }

    // Usamos OnTriggerEnter porque la bala es un "Trigger"
    void OnTriggerEnter(Collider otro)
    {
        // Revisamos si chocamos con algo que tenga vida
        VidaEnemigo enemigo = otro.GetComponent<VidaEnemigo>();

        // Si tiene el script de vida, le quitamos 1 punto
        if (enemigo != null)
        {
            enemigo.RecibirDanio(1);
        }

        // La bala siempre se destruye al impactar cualquier cosa
        Destroy(gameObject);
    }
}