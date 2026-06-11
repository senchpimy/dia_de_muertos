using UnityEngine;
using UnityEngine.InputSystem;

public class CargarObjetos : MonoBehaviour
{
    public Transform puntoDeAgarre;
    public float distanciaDeAgarre = 2.0f;

    private GameObject objetoCargado;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("1. Se presion la tecla E en el teclado!");

            if (objetoCargado == null)
            {
                IntentarAgarrar();
            }
            else
            {
                SoltarObjeto();
            }
        }
    }

    void IntentarAgarrar()
    {
        Vector3 origenRayo = transform.position + Vector3.up * 0.5f;
        RaycastHit golpe;

        Debug.DrawRay(origenRayo, transform.forward * distanciaDeAgarre, Color.red, 2f);

        if (Physics.Raycast(origenRayo, transform.forward, out golpe, distanciaDeAgarre))
        {
            Debug.Log("El rayo chocó con: " + golpe.collider.name);

            if (golpe.collider.CompareTag("Cargable"))
            {
                objetoCargado = golpe.collider.gameObject;

                Rigidbody rb = objetoCargado.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                objetoCargado.transform.position = puntoDeAgarre.position;
                objetoCargado.transform.SetParent(puntoDeAgarre);

                objetoCargado.layer = 2;
                foreach (Transform child in objetoCargado.GetComponentsInChildren<Transform>(true))
                {
                    child.gameObject.layer = 2;
                }

                Collider col = objetoCargado.GetComponent<Collider>();
                if (col != null) col.isTrigger = true;
            }
            else
            {
            }
        }
        else
        {
            Debug.Log("ERROR: El rayo no choco");
        }
    }

    void SoltarObjeto()
    {
        Debug.Log("Soltando el objeto...");
        Rigidbody rb = objetoCargado.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        objetoCargado.layer = 0;
        foreach (Transform child in objetoCargado.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = 0;
        }

        Collider col = objetoCargado.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        objetoCargado.transform.SetParent(null);
        objetoCargado = null;
    }
}
