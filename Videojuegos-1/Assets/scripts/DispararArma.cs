using UnityEngine;
using System.Collections;

public class DispararArma : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject balaPrefab;
    public Transform puntoDisparo;
    public float fuerza = 40f;

    [Header("Munición")]
    public int municionActual = 15;
    public int municionMaxima = 15;
    public float tiempoRecarga = 2f;

    [Header("Estado")]
    public bool estaRecargando = false;

    private Camera camaraPrincipal;

    void Start()
    {
        // Aseguramos detectar la cámara marcada como 'MainCamera'
        camaraPrincipal = Camera.main;
    }

    void Update()
    {
        // 1. Si está recargando, bloqueamos cualquier disparo
        if (estaRecargando) return;

        // 2. Disparar con Clic Izquierdo (Mouse 0)
        if (Input.GetMouseButtonDown(0) && municionActual > 0)
        {
            Disparar();
        }
        else if (Input.GetMouseButtonDown(0) && municionActual <= 0)
        {
            Debug.Log("¡Sin munición! Presiona R para recargar.");
        }

        // 3. Recargar con la tecla R
        if (Input.GetKeyDown(KeyCode.R) && municionActual < municionMaxima && !estaRecargando)
        {
            StartCoroutine(Recargar());
        }
    }

    void Disparar()
    {
        municionActual--;

        // Creamos el rayo desde el centro de la pantalla
        Ray rayo = camaraPrincipal.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit golpe;
        Vector3 puntoDestino;

        if (Physics.Raycast(rayo, out golpe))
        {
            puntoDestino = golpe.point;
        }
        else
        {
            puntoDestino = rayo.GetPoint(1000); // Disparo lejano si no hay obstáculos
        }

        Vector3 direccionDisparo = (puntoDestino - puntoDisparo.position).normalized;

        // Instanciamos y disparamos la bala
        GameObject nuevaBala = Instantiate(balaPrefab, puntoDisparo.position, Quaternion.LookRotation(direccionDisparo));

        Rigidbody rb = nuevaBala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direccionDisparo * fuerza;
        }

        Destroy(nuevaBala, 3f);
    }

    IEnumerator Recargar()
    {
        estaRecargando = true;
        Debug.Log("Recargando...");

        yield return new WaitForSeconds(tiempoRecarga);

        municionActual = municionMaxima;
        estaRecargando = false;
        Debug.Log("Recarga completada.");
    }
}