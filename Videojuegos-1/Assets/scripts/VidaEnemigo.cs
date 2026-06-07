using UnityEngine;
using System.Collections; // Necesario para contar los segundos

public class VidaEnemigo : MonoBehaviour
{
    [Header("Configuraci�n de Vida")]
    public int vidaMaxima = 3;
    private int vidaActual;

    // Guardaremos d�nde inicia para que reviva en ese mismo lugar
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Awake()
    {
        // Eliminamos al enemigo apenas aparece en el juego
        Destroy(gameObject);
    }

    void Start()
    {
        vidaActual = vidaMaxima;
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    // Esta funci�n la llama la bala cuando lo toca
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

        // 1. Apagamos el script que le da la orden de seguirte para evitar el error "SetDestination"
        GetComponent<SeguirJugador>().enabled = false;

        // 2. Apagamos su "cerebro" (NavMesh) y su "cuerpo" (Collider) 
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // 3. Lo escondemos (lo movemos 100 metros debajo del piso)
        transform.position = new Vector3(transform.position.x, -100f, transform.position.z);

        // 4. AHORA S�: Esperamos 10 segundos. 
        yield return new WaitForSeconds(10f);

        // 5. Lo regresamos a su lugar de origen y restauramos su vida
        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;
        vidaActual = vidaMaxima;

        // 6. Volvemos a prender su cerebro y su cuerpo f�sico
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        GetComponent<Collider>().enabled = true;

        // 7. Volvemos a prender su instinto de persecuci�n (el script)
        GetComponent<SeguirJugador>().enabled = true;

        Debug.Log("�El esqueleto ha revivido!");
    }
}