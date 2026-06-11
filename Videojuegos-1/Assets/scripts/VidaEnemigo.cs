using UnityEngine;
using System.Collections; 

public class VidaEnemigo : MonoBehaviour
{
    [Header("Configuraci�n de Vida")]
    public int vidaMaxima = 3;
    private int vidaActual;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        vidaActual = vidaMaxima;
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    public void RecibirDanio(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("�Au! Vida restante del esqueleto: " + vidaActual);

        if (vidaActual <= 0)
        {
            StartCoroutine(MorirYRevivir());
        }
    }

    IEnumerator MorirYRevivir()
    {
        Debug.Log("Esqueleto derrotado. Esperando 10 segundos...");

        GetComponent<SeguirJugador>().enabled = false;

        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        GetComponent<Collider>().enabled = false;

        transform.position = new Vector3(transform.position.x, -100f, transform.position.z);

        yield return new WaitForSeconds(10f);

        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
        vidaActual = vidaMaxima;

        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        GetComponent<Collider>().enabled = true;

        GetComponent<SeguirJugador>().enabled = true;

        Debug.Log("�El esqueleto ha revivido!");
    }
}
