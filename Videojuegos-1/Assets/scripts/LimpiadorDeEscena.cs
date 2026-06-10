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
        // 1. Esperar un poco para que Unity procese el GLB
        yield return new WaitForSeconds(0.2f);

        Debug.Log("--- EJECUTANDO LIMPIEZA TOTAL ---");

        // 2. Crear la plataforma de emergencia
        GameObject plataforma = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plataforma.name = "SUELO_SEGURO";
        plataforma.transform.position = new Vector3(0, -0.5f, 0);
        plataforma.transform.localScale = new Vector3(200, 1, 200);
        plataforma.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);

        // 3. Crear Texto 3D (RESTAURADO)
        GameObject texto3D = new GameObject("Texto3D_Ejemplo");
        TextMesh tm = texto3D.AddComponent<TextMesh>();
        tm.text = "¡BIENVENIDO AL ESPACIO VACÍO!\nRecoge los Panes de Muerto con 'E'.";
        tm.fontSize = 50;
        tm.characterSize = 0.2f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        texto3D.transform.position = new Vector3(0, 5, 10);
        texto3D.transform.localScale = new Vector3(1, 1, 1);

        // 4. Localizar al jugador
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) jugador = GameObject.Find("PlayerArmature");
        
        if (jugador != null)
        {
            jugador.transform.position = new Vector3(0, 2f, 0);
        }

        // 5. Limpiar el resto de la escena
        GameObject[] todos = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in todos)
        {
            if (obj == null || obj == plataforma || obj == texto3D || obj == gameObject || obj.transform.root == transform) continue;
            if (jugador != null && (obj == jugador || obj.transform.IsChildOf(jugador.transform))) continue;
            
            if (obj.GetComponent<Camera>() != null || 
                obj.GetComponent<Light>() != null || 
                obj.GetComponent<Canvas>() != null ||
                obj.name.Contains("Camera") ||
                obj.name.Contains("EventSystem") ||
                obj.name.Contains("Configuracion")) continue;

            Destroy(obj);
        }

        // 6. GENERAR PANES DE MUERTO (MODELO GLB)
        // Nota: Si sigue saliendo esferas, Unity no ha importado el .glb como GameObject an.
        GameObject prefabPan = Resources.Load<GameObject>("pan_de_muerto");
        
        if (prefabPan == null)
        {
            Debug.LogWarning("Resources.Load no pudo encontrar 'pan_de_muerto'. Asegúrate de que el archivo .glb haya terminado de importarse en el editor.");
        }

        for (int i = 0; i < 8; i++)
        {
            GameObject pan;
            if (prefabPan != null)
            {
                pan = GameObject.Instantiate(prefabPan);
                pan.name = "PanDeMuerto_" + i;
            }
            else
            {
                pan = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pan.name = "Esfera_TEMP_Pan_" + i;
                pan.GetComponent<Renderer>().material.color = new Color(0.8f, 0.4f, 0.1f);
            }

            pan.transform.position = new Vector3(Random.Range(-15, 15), 1, Random.Range(10, 25));
            pan.transform.localScale = Vector3.one * 1.5f;
            pan.tag = "Cargable";
            
            if (pan.GetComponent<Rigidbody>() == null) pan.AddComponent<Rigidbody>();
            
            if (pan.GetComponent<Collider>() == null)
            {
                MeshRenderer mr = pan.GetComponentInChildren<MeshRenderer>();
                if (mr != null) 
                {
                    BoxCollider bc = pan.AddComponent<BoxCollider>();
                    bc.center = pan.transform.InverseTransformPoint(mr.bounds.center);
                    bc.size = mr.bounds.size;
                }
                else
                {
                    pan.AddComponent<SphereCollider>();
                }
            }
        }

        Debug.Log("--- ESCENA LIMPIA, TEXTO RESTAURADO Y PANES LISTOS ---");
    }
}
