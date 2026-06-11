using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    public int danio = 1;
    public float tiempoEntreAtaques = 1.5f;
    private float tiempoUltimoAtaque;

    void OnTriggerStay(Collider otro)
    {
        if (otro.CompareTag("Player"))
        {
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
