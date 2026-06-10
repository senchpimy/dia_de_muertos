using UnityEngine;
using UnityEngine.InputSystem;

public class CargarObjetos : MonoBehaviour
{
    public Transform puntoDeAgarre;
    public float distanciaDeAgarre = 2.0f;

    private GameObject objetoCargado;

    void Update()
    {
        // 1. Verificamos si detecta la tecla
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("1. �Se presion� la tecla E en el teclado!");

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
        // Bajamos el origen del rayo a la altura de las rodillas/cintura (0.5f en lugar de 1.2f)
        Vector3 origenRayo = transform.position + Vector3.up * 0.5f;
        RaycastHit golpe;

        Debug.DrawRay(origenRayo, transform.forward * distanciaDeAgarre, Color.red, 2f);

        // 2. Verificamos si el rayo golpea CUALQUIER cosa
        if (Physics.Raycast(origenRayo, transform.forward, out golpe, distanciaDeAgarre))
        {
            Debug.Log("2. El rayo choc� con un objeto llamado: " + golpe.collider.name);

            // 3. Verificamos si el objeto tiene la etiqueta correcta
            if (golpe.collider.CompareTag("Cargable"))
            {
                Debug.Log("3. ¡Etiqueta Cargable detectada! Levantando el Pan de Muerto...");
                objetoCargado = golpe.collider.gameObject;

                Rigidbody rb = objetoCargado.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                objetoCargado.transform.position = puntoDeAgarre.position;
                objetoCargado.transform.SetParent(puntoDeAgarre);
            }
            else
            {
                Debug.Log("3. ERROR: Choc� con algo, pero NO tiene la etiqueta 'Cargable'.");
            }
        }
        else
        {
            Debug.Log("2. ERROR: El rayo no choc� con nada. Ac�rcate m�s o ajusta la altura.");
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

        objetoCargado.transform.SetParent(null);
        objetoCargado = null;
    }
}