using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public int danio = 1;
    public float tiempoEntreAtaques = 1.5f; // Segundos entre cada golpe
    private float tiempoUltimoAtaque;

    // ¡Cambiamos OnCollisionStay por OnTriggerStay!
    void OnTriggerStay(Collider otro)
    {
        // 1. Verificamos si lo que entró al aura es el jugador
        if (otro.CompareTag("Player"))
        {
            // 2. Controlamos el cronómetro de golpes
            if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
            {
                VidaJugador jugador = otro.GetComponent<VidaJugador>();

                if (jugador != null)
                {
                    jugador.RecibirDanio(danio);
                    tiempoUltimoAtaque = Time.time; // Reiniciamos el tiempo
                }
            }
        }
    }
}