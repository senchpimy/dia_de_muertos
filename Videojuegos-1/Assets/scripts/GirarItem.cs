using UnityEngine;

public class GirarItem : MonoBehaviour
{
    public float velocidadGiro = 100f; // Qué tan rápido da vueltas

    void Update()
    {
        // Gira este objeto sobre el eje Y (vertical) cada segundo
        transform.Rotate(Vector3.up * velocidadGiro * Time.deltaTime);
    }
}