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
        camaraPrincipal = Camera.main;
    }

    void Update()
    {
        if (estaRecargando) return;

        if (Input.GetMouseButtonDown(0) && municionActual > 0)
        {
            Disparar();
        }
        else if (Input.GetMouseButtonDown(0) && municionActual <= 0)
        {
            Debug.Log("¡Sin munición! Presiona R para recargar.");
        }

        if (Input.GetKeyDown(KeyCode.R) && municionActual < municionMaxima && !estaRecargando)
        {
            StartCoroutine(Recargar());
        }
    }

    void Disparar()
    {
        municionActual--;

        Ray rayo = camaraPrincipal.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit golpe;
        Vector3 puntoDestino;

        if (Physics.Raycast(rayo, out golpe))
        {
            puntoDestino = golpe.point;
        }
        else
        {
            puntoDestino = rayo.GetPoint(1000);
        }

        Vector3 direccionDisparo = (puntoDestino - puntoDisparo.position).normalized;

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
