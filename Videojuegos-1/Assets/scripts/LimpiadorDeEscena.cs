using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using GLTFast;

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
            StopAllCoroutines();
            StartCoroutine(ForzarPlataformaYLimpieza());
        }
    }

    private bool EsEscenaDeJuego(string nombre)
    {
        return !nombre.Contains("Menu") && !nombre.Contains("Creditos") && !nombre.Contains("Selector");
    }

    IEnumerator ForzarPlataformaYLimpieza()
    {
        // 1. Crear la plataforma de inmediato para que nadie caiga
        GameObject plataforma = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plataforma.name = "SUELO_SEGURO_ESTABLE";
        plataforma.transform.position = new Vector3(0, -0.5f, 0);
        plataforma.transform.localScale = new Vector3(200, 1, 200);
        plataforma.GetComponent<Renderer>().material.color = new Color(0.15f, 0.15f, 0.15f);
        
        // Evitar que la plataforma se borre a s misma
        Rigidbody platRb = plataforma.AddComponent<Rigidbody>();
        platRb.isKinematic = true;

        // 2. Crear Texto 3D (RESTAURADO)
        GameObject texto3D = new GameObject("Texto3D_Informativo");
        TextMesh tm = texto3D.AddComponent<TextMesh>();
        tm.text = "¡BIENVENIDO AL ESPACIO VACÍO!\nRecoge los Panes de Muerto con 'E'.";
        tm.fontSize = 60;
        tm.characterSize = 0.2f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        texto3D.transform.position = new Vector3(0, 6, 15);

        yield return new WaitForFixedUpdate();

        // 3. Localizar al jugador y sus dependencias crticas
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) jugador = GameObject.FindAnyObjectByType<CharacterController>()?.gameObject;
        
        List<GameObject> protegidos = new List<GameObject> { plataforma, texto3D, gameObject };
        
        if (jugador != null)
        {
            Debug.Log("Jugador detectado: " + jugador.name);
            protegidos.Add(jugador);
            // Proteger todos los hijos del jugador (cmaras, huesos, etc)
            foreach (Transform t in jugador.GetComponentsInChildren<Transform>(true))
                protegidos.Add(t.gameObject);
            
            // Proteger cmara de seguimiento si est afuera
            GameObject cam = GameObject.Find("PlayerFollowCamera");
            if (cam != null) protegidos.Add(cam);

            // Posicionar al jugador con cuidado
            var controller = jugador.GetComponent<CharacterController>();
            var tpControl = jugador.GetComponent("ThirdPersonController"); // Usamos string por si acaso

            if (controller != null) controller.enabled = false;
            if (tpControl != null) (tpControl as MonoBehaviour).enabled = false;

            jugador.transform.position = new Vector3(0, 1.5f, 0);
            
            yield return null;
            
            if (controller != null) controller.enabled = true;
            if (tpControl != null) (tpControl as MonoBehaviour).enabled = true;
        }

        // 4. Limpieza quirrgica de la escena
        GameObject[] todos = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in todos)
        {
            if (obj == null) continue;
            if (protegidos.Contains(obj)) continue;
            
            // No borrar cmaras principales, luces, UI o sistemas de entrada
            if (obj.GetComponent<Camera>() != null || 
                obj.GetComponent<Light>() != null || 
                obj.name.Contains("Canvas") || 
                obj.name.Contains("EventSystem") ||
                obj.name.Contains("Input") ||
                obj.name.Contains("MainCamera")) continue;

            Destroy(obj);
        }

        // 5. CARGAR PAN DE MUERTO
        TextAsset glbData = Resources.Load<TextAsset>("pan_de_muerto");
        if (glbData != null)
        {
            for (int i = 0; i < 12; i++)
            {
                StartCoroutine(InstanciarPan(glbData.bytes, i));
            }
        }
        else
        {
            Debug.LogError("No se encontr el archivo pan_de_muerto.bytes en Resources.");
        }
    }

    IEnumerator InstanciarPan(byte[] bytes, int index)
    {
        var gltf = new GltfImport();
        var success = gltf.LoadGltfBinary(bytes);
        while (!success.IsCompleted) yield return null;

        if (success.Result)
        {
            GameObject container = new GameObject("Pan_" + index);
            container.transform.position = new Vector3(Random.Range(-25, 25), 2, Random.Range(10, 40));
            container.transform.localScale = Vector3.one * 3f; // Un poco ms grandes para verlos bien
            container.tag = "Cargable";

            var inst = gltf.InstantiateMainSceneAsync(container.transform);
            while (!inst.IsCompleted) yield return null;

            // Fsicas y Colisin
            Rigidbody rb = container.AddComponent<Rigidbody>();
            rb.mass = 2f;
            
            // Intentar poner un collider preciso
            MeshRenderer mr = container.GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                BoxCollider bc = container.AddComponent<BoxCollider>();
                bc.center = container.transform.InverseTransformPoint(mr.bounds.center);
                bc.size = mr.bounds.size;
            }
            else
            {
                container.AddComponent<SphereCollider>();
            }
            
            Debug.Log("Pan " + index + " aparecido en pantalla.");
        }
    }
}
