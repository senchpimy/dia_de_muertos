using UnityEngine;
using UnityEngine.AI;

public class SeguirJugador : MonoBehaviour
{
    public Transform objetivo;
    private NavMeshAgent agente;
    private Animator anim; // Añadimos el componente Animator

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>(); // Obtenemos el animator
    }

    void Update()
    {
        if (objetivo != null)
        {
            agente.SetDestination(objetivo.position);

            // Le pasamos la velocidad al animator para que sepa qué animación usar
            anim.SetFloat("Speed", agente.velocity.magnitude);
        }
    }
}