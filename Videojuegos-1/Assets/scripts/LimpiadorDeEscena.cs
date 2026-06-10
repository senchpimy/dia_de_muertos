using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using GLTFast;
using TMPro;
using UnityEngine.UI;

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
        if (EsEscenaDeJuego(escena.name) || escena.name == "EscenarioCalavera")
        {
            StopAllCoroutines();
            StartCoroutine(ForzarPlataformaYLimpieza(escena.name));
            StartCoroutine(LimpiadorPersistente());
        }
    }

    private bool EsEscenaDeJuego(string nombre)
    {
        return !nombre.Contains("Menu") && !nombre.Contains("Creditos") && !nombre.Contains("Selector");
    }

    IEnumerator LimpiadorPersistente()
    {
        for (int i = 0; i < 5; i++)
        {
            EjecutarLimpiezaDeTextos();
            yield return new WaitForSeconds(1f);
        }
    }

    void EjecutarLimpiezaDeTextos()
    {
        GameObject[] todos = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in todos)
        {
            if (obj == null || obj.scene.name == null) continue;

            string nombre = obj.name.ToUpper();
            if (nombre.Contains("MISION") || nombre.Contains("ESCAPE") || nombre.Contains("CAJA") || nombre.Contains("TUTORIAL"))
            {
                if (obj.activeInHierarchy) Destroy(obj);
                else obj.SetActive(false);
                continue;
            }

            Text legacyText = obj.GetComponent<Text>();
            if (legacyText != null && TextoContieneMision(legacyText.text)) { Destroy(obj); continue; }

            TMP_Text tmpText = obj.GetComponent<TMP_Text>();
            if (tmpText != null && TextoContieneMision(tmpText.text)) { Destroy(obj); continue; }

            TextMesh tm = obj.GetComponent<TextMesh>();
            if (tm != null && TextoContieneMision(tm.text)) { Destroy(obj); continue; }
        }
    }

    bool TextoContieneMision(string t)
    {
        if (string.IsNullOrEmpty(t)) return false;
        string upper = t.ToUpper();
        return upper.Contains("MISI") || upper.Contains("ESCAPE") || upper.Contains("CAJA");
    }

    IEnumerator ForzarPlataformaYLimpieza(string nombreEscena)
    {
        yield return new WaitForSeconds(0.05f);

        GameObject plataforma = null;
        if (nombreEscena != "EscenarioCalavera")
        {
            plataforma = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plataforma.name = "SUELO_SEGURO_ESTABLE";
            plataforma.transform.position = new Vector3(0, -0.5f, 0);
            plataforma.transform.localScale = new Vector3(200, 1, 200);
            plataforma.GetComponent<Renderer>().material.color = new Color(0.15f, 0.15f, 0.15f);
            plataforma.layer = 0; 
        }

        GameObject texto3D = new GameObject("Texto3D_Informativo");
        TextMesh tm = texto3D.AddComponent<TextMesh>();
        tm.text = nombreEscena == "EscenarioCalavera" ? "¡EXPLORA LA GRAN CALAVERA!" : "¡BIENVENIDO AL ESPACIO VACÍO!\nRecoge los Panes de Muerto con 'E'.";
        tm.fontSize = 60;
        tm.characterSize = 0.2f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        texto3D.transform.position = new Vector3(0, 10, 20);

        // --- NUEVO: CREAR BOTÓN DE TELETRANSPORTE EN NIVEL 1 ---
        GameObject botonPortal = null;
        if (nombreEscena == "Nivel 1")
        {
            botonPortal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            botonPortal.name = "BOTON_PORTAL_CALAVERA";
            botonPortal.transform.position = new Vector3(10, 0.1f, 0); // Al lado del jugador
            botonPortal.transform.localScale = new Vector3(2, 0.1f, 2);
            botonPortal.GetComponent<Renderer>().material.color = Color.magenta;
            
            // Hacerlo Trigger para que el script lo detecte al pisarlo
            botonPortal.GetComponent<Collider>().isTrigger = true;
            botonPortal.AddComponent<BotonTeletransporte>();

            // Agregar un texto flotante sobre el botón
            GameObject textoBoton = new GameObject("TextoBoton");
            textoBoton.transform.SetParent(botonPortal.transform);
            textoBoton.transform.localPosition = new Vector3(0, 10, 0); // Relativo al botón
            TextMesh tmBoton = textoBoton.AddComponent<TextMesh>();
            tmBoton.text = "PÍSAME PARA IR\nA LA CALAVERA";
            tmBoton.fontSize = 40;
            tmBoton.anchor = TextAnchor.MiddleCenter;
            tmBoton.alignment = TextAlignment.Center;
            tmBoton.color = Color.magenta;
            textoBoton.transform.localScale = new Vector3(0.5f, 5, 0.5f); // Ajustar por escala del cilindro
        }
        // -------------------------------------------------------

        yield return new WaitForFixedUpdate();

        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) jugador = GameObject.FindAnyObjectByType<CharacterController>()?.gameObject;
        
        HashSet<GameObject> protegidos = new HashSet<GameObject> { texto3D, gameObject };
        if (plataforma != null) protegidos.Add(plataforma);
        if (botonPortal != null) {
            protegidos.Add(botonPortal);
            foreach (Transform t in botonPortal.GetComponentsInChildren<Transform>(true)) protegidos.Add(t.gameObject);
        }
        
        if (jugador != null)
        {
            protegidos.Add(jugador);
            foreach (Transform t in jugador.GetComponentsInChildren<Transform>(true))
                protegidos.Add(t.gameObject);
            
            GameObject cam = GameObject.Find("PlayerFollowCamera");
            if (cam != null) protegidos.Add(cam);
            
            VidaJugador vida = jugador.GetComponent<VidaJugador>();
            if (vida != null && vida.barraDeVidaUI != null)
            {
                GameObject rootUI = vida.barraDeVidaUI.transform.root.gameObject;
                protegidos.Add(rootUI);
                foreach (Transform t in rootUI.GetComponentsInChildren<Transform>(true)) protegidos.Add(t.gameObject);
            }

            var controller = jugador.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            
            jugador.transform.position = nombreEscena == "EscenarioCalavera" ? new Vector3(0, 50, 0) : new Vector3(0, 1.5f, 0);
            
            yield return null;
            if (controller != null) controller.enabled = true;
        }

        EjecutarLimpiezaDeTextos();

        GameObject[] todosActivos = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in todosActivos)
        {
            if (obj == null || protegidos.Contains(obj)) continue;
            if (obj.GetComponent<Camera>() != null || obj.GetComponent<Light>() != null || 
                obj.name.Contains("Input") || obj.name.Contains("EventSystem") ||
                obj.name.Contains("MainCamera")) continue;
            Destroy(obj);
        }

        if (nombreEscena == "EscenarioCalavera")
        {
            StartCoroutine(CargarMapaCalavera());
        }
        else
        {
            TextAsset glbData = Resources.Load<TextAsset>("pan_de_muerto");
            if (glbData != null)
            {
                for (int i = 0; i < 12; i++)
                {
                    StartCoroutine(InstanciarPan(glbData.bytes, i));
                }
            }
        }
    }

    IEnumerator CargarMapaCalavera()
    {
        TextAsset glbData = Resources.Load<TextAsset>("skull_dia_de_muertos");
        if (glbData == null) { Debug.LogError("No se encontró skull_dia_de_muertos.bytes"); yield break; }

        var gltf = new GltfImport();
        var success = gltf.LoadGltfBinary(glbData.bytes);
        while (!success.IsCompleted) yield return null;

        if (success.Result)
        {
            GameObject mapa = new GameObject("MAPA_CALAVERA");
            mapa.transform.position = Vector3.zero;
            mapa.transform.localScale = Vector3.one * 50f;

            var inst = gltf.InstantiateMainSceneAsync(mapa.transform);
            while (!inst.IsCompleted) yield return null;

            foreach (var cam in mapa.GetComponentsInChildren<Camera>(true)) Destroy(cam.gameObject);

            foreach (var mr in mapa.GetComponentsInChildren<MeshRenderer>(true))
            {
                mr.gameObject.AddComponent<MeshCollider>();
            }
            
            Debug.Log("Mapa Calavera cargado con éxito.");
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
            container.transform.localScale = Vector3.one * 3f;
            container.tag = "Cargable";

            var inst = gltf.InstantiateMainSceneAsync(container.transform);
            while (!inst.IsCompleted) yield return null;

            Component[] componentes = container.GetComponentsInChildren<Component>(true);
            foreach (var comp in componentes)
            {
                if (comp == null || comp is Transform || comp is MeshRenderer || comp is MeshFilter || comp is SkinnedMeshRenderer) 
                    continue;
                if (comp is Camera || comp is AudioListener || comp.GetType().Name.Contains("Cinemachine"))
                    Destroy(comp);
            }

            Rigidbody rb = container.AddComponent<Rigidbody>();
            rb.mass = 2f;
            
            MeshRenderer mr = container.GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                BoxCollider bc = container.AddComponent<BoxCollider>();
                bc.center = container.transform.InverseTransformPoint(mr.bounds.center);
                bc.size = mr.bounds.size;
                bc.isTrigger = false;
            }
            else
            {
                container.AddComponent<SphereCollider>();
            }
        }
    }
}
