using UnityEngine;

public class GirarItem : MonoBehaviour
{
    public float velocidadGiro = 100f;

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadGiro * Time.deltaTime);
    }
}
