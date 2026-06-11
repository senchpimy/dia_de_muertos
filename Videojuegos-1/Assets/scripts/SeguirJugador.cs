using UnityEngine;
using UnityEngine.AI;

public class SeguirJugador : MonoBehaviour
{
    public Transform objetivo;
    public float velocidadManual = 1.5f;
    private NavMeshAgent agente;
    private Animator anim;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        
        if (agente != null && !agente.isOnNavMesh)
        {
            agente.enabled = false;
        }
    }

    void Update()
    {
        if (objetivo == null) return;

        if (agente != null && agente.enabled && agente.isOnNavMesh)
        {
            agente.SetDestination(objetivo.position);
            if (anim != null) anim.SetFloat("Speed", agente.velocity.magnitude);
        }
        else
        {
            MoverManualmente();
        }
    }

    void MoverManualmente()
    {
        Vector3 direccion = (objetivo.position - transform.position).normalized;
        direccion.y = 0; // Mantenerse derecho
        
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
        }

        transform.Translate(Vector3.forward * velocidadManual * Time.deltaTime);

        if (anim != null) anim.SetFloat("Speed", velocidadManual);
    }
}
