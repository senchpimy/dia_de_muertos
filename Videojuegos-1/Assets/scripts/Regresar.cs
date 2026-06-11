using UnityEngine;
using UnityEngine.SceneManagement;

public class Regresar : MonoBehaviour
{
    public void RegresarMenu()
    {
        SceneManager.LoadScene("MenuMain");
    }
}
