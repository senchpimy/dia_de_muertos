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
        if (EsEscenaDeJuego(escena.name))
        {
            StopAllCoroutines();
            StartCoroutine(ForzarPlataformaYLimpieza());
            // Lanzamos una rutina extra que limpia cada segundo por si algo reaparece
            StartCoroutine(LimpiadorPersistente());
        }
    }

    private bool EsEscenaDeJuego(string nombre)
    {
        return !nombre.Contains("Menu") && !nombre.Contains("Creditos") && !nombre.Contains("Selector");
    }

    IEnumerator LimpiadorPersistente()
    {
        // Durante los primeros 5 segundos del nivel, limpiamos cada segundo
        // por si algn script del juego original intenta reactivar la interfaz vieja.
        for (int i = 0; i < 5; i++)
        {
            EjecutarLimpiezaDeTextos();
            yield return new WaitForSeconds(1f);
        }
    }

    void EjecutarLimpiezaDeTextos()
    {
        // Buscamos TODOS los objetos, incluso los desactivados
        GameObject[] todos = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in todos)
        {
            if (obj == null || obj.scene.name == null) continue; // Ignorar prefabs en la carpeta Assets

            string nombre = obj.name.ToUpper();
            if (nombre.Contains("MISION") || nombre.Contains("ESCAPE") || nombre.Contains("CAJA") || nombre.Contains("TUTORIAL"))
            {
                // Si el objeto se llama as, lo borramos o desactivamos
                if (obj.activeInHierarchy) Destroy(obj);
                else obj.SetActive(false); // Si ya est desactivado, nos aseguramos
                continue;
            }

            // Tambin revisamos el contenido de los textos
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

    IEnumerator ForzarPlataformaYLimpieza()
    {
        yield return new WaitForSeconds(0.05f);

        // 1. Crear plataforma estable
        GameObject plataforma = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plataforma.name = "SUELO_SEGURO_ESTABLE";
        plataforma.transform.position = new Vector3(0, -0.5f, 0);
        plataforma.transform.localScale = new Vector3(200, 1, 200);
        plataforma.GetComponent<Renderer>().material.color = new Color(0.15f, 0.15f, 0.15f);
        plataforma.layer = 0; 

        // 2. Nuevo Texto 3D de Bienvenida
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

        // 3. Localizar al jugador y protegerlo
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) jugador = GameObject.FindAnyObjectByType<CharacterController>()?.gameObject;
        
        HashSet<GameObject> protegidos = new HashSet<GameObject> { plataforma, texto3D, gameObject };
        
        if (jugador != null)
        {
            protegidos.Add(jugador);
            foreach (Transform t in jugador.GetComponentsInChildren<Transform>(true))
                protegidos.Add(t.gameObject);
            
            GameObject cam = GameObject.Find("PlayerFollowCamera");
            if (cam != null) protegidos.Add(cam);
            
            // Proteger UI de vida
            VidaJugador vida = jugador.GetComponent<VidaJugador>();
            if (vida != null && vida.barraDeVidaUI != null)
            {
                GameObject rootUI = vida.barraDeVidaUI.transform.root.gameObject;
                protegidos.Add(rootUI);
                foreach (Transform t in rootUI.GetComponentsInChildren<Transform>(true)) protegidos.Add(t.gameObject);
            }

            var controller = jugador.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            jugador.transform.position = new Vector3(0, 1.5f, 0);
            yield return null;
            if (controller != null) controller.enabled = true;
        }

        // 4. LIMPIEZA TOTAL
        // Primero ejecutamos la limpieza de textos específica
        EjecutarLimpiezaDeTextos();

        // Luego borramos el resto de objetos decorativos/laberinto
        GameObject[] todosActivos = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in todosActivos)
        {
            if (obj == null || protegidos.Contains(obj)) continue;

            // No borrar sistemas vitales
            if (obj.GetComponent<Camera>() != null || 
                obj.GetComponent<Light>() != null || 
                obj.name.Contains("Input") || 
                obj.name.Contains("EventSystem") ||
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

            // Limpieza de cámaras internas
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
            }
            else
            {
                container.AddComponent<SphereCollider>();
            }
        }
    }
}
