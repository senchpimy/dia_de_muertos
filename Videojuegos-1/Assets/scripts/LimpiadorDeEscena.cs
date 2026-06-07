using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LimpiadorDeEscena : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (EsEscenaDeJuego(escena.name))
        {
            StartCoroutine(ForzarPlataformaYLimpieza());
        }
    }

    private bool EsEscenaDeJuego(string nombre)
    {
        return !nombre.Contains("Menu") && !nombre.Contains("Creditos") && !nombre.Contains("Selector");
    }

    IEnumerator ForzarPlataformaYLimpieza()
    {
        // Esperar a que el motor de fsica y el personaje se asienten
        yield return new WaitForFixedUpdate();

        Debug.Log("--- EJECUTANDO LIMPIEZA TOTAL ---");

        // 1. Crear la plataforma de emergencia de inmediato
        GameObject plataforma = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plataforma.name = "SUELO_SEGURO";
        plataforma.transform.position = new Vector3(0, -0.5f, 0);
        plataforma.transform.localScale = new Vector3(200, 1, 200);
        
        // Material visible para debug (Gris oscuro)
        plataforma.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);

        // 2. Localizar y salvar al jugador
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) jugador = GameObject.Find("PlayerArmature");
        if (jugador == null) jugador = GameObject.Find("PlayerCapsule");

        if (jugador != null)
        {
            // Desactivar fsicas temporalmente para reposicionar
            CharacterController cc = jugador.GetComponent<CharacterController>();
            Rigidbody rb = jugador.GetComponent<Rigidbody>();
            
            if (cc != null) cc.enabled = false;
            if (rb != null) { rb.linearVelocity = Vector3.zero; rb.isKinematic = true; }

            jugador.transform.position = new Vector3(0, 2f, 0);
            Debug.Log("Jugador posicionado en el centro.");

            yield return new WaitForEndOfFrame();

            if (cc != null) cc.enabled = true;
            if (rb != null) rb.isKinematic = false;
        }

        // 3. Limpiar el resto de la escena
        GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in todos)
        {
            if (obj == null || obj == plataforma || obj == gameObject || obj.transform.root == transform) continue;
            
            // Excluir jugador y sus componentes hijos
            if (jugador != null && (obj == jugador || obj.transform.IsChildOf(jugador.transform))) continue;

            // Excluir elementos vitales
            if (obj.GetComponent<Camera>() != null || 
                obj.GetComponent<Light>() != null || 
                obj.GetComponent<Canvas>() != null ||
                obj.name.Contains("Camera") ||
                obj.name.Contains("EventSystem") ||
                obj.name.Contains("Configuracion") ||
                obj.name.Contains("Controller") ||
                obj.name.Contains("Input")) continue;

            // Destruir todo lo dems (Laberintos, enemigos, tutoriales, etc.)
            Destroy(obj);
        }

        Debug.Log("--- ESCENA LIMPIA Y SEGURA ---");
    }
}
