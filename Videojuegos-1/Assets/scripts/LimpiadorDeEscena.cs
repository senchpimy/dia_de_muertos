using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using GLTFast;
using TMPro; // Por si el texto usa TextMeshPro

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
        yield return new WaitForSeconds(0.1f);

        // 1. Crear plataforma estable (CAPA DEFAULT para que el personaje detecte suelo)
        GameObject plataforma = GameObject.CreatePrimitive(PrimitiveType.Cube);
        plataforma.name = "SUELO_SEGURO_ESTABLE";
        plataforma.transform.position = new Vector3(0, -0.5f, 0);
        plataforma.transform.localScale = new Vector3(200, 1, 200);
        plataforma.GetComponent<Renderer>().material.color = new Color(0.15f, 0.15f, 0.15f);
        plataforma.layer = 0; // Layer Default

        // 2. Nuevo Texto 3D
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

        // 3. Localizar al jugador
        GameObject jugador = GameObject.FindWithTag("Player");
        if (jugador == null) jugador = GameObject.FindAnyObjectByType<CharacterController>()?.gameObject;
        
        List<GameObject> protegidos = new List<GameObject> { plataforma, texto3D, gameObject };
        
        if (jugador != null)
        {
            protegidos.Add(jugador);
            foreach (Transform t in jugador.GetComponentsInChildren<Transform>(true))
                protegidos.Add(t.gameObject);
            
            GameObject cam = GameObject.Find("PlayerFollowCamera");
            if (cam != null) protegidos.Add(cam);

            var controller = jugador.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            jugador.transform.position = new Vector3(0, 1.5f, 0);
            yield return null;
            if (controller != null) controller.enabled = true;
        }

        // 4. Limpieza de escena
        GameObject[] todos = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in todos)
        {
            if (obj == null || protegidos.Contains(obj)) continue;
            
            // Eliminar especficamente el texto de la misin vieja si aparece
            TextMesh tmOld = obj.GetComponent<TextMesh>();
            if (tmOld != null && (tmOld.text.ToUpper().Contains("CAJAS") || tmOld.text.ToUpper().Contains("ESCAPE") || tmOld.text.ToUpper().Contains("MISI"))) { Destroy(obj); continue; }
            
            TMP_Text tmpOld = obj.GetComponent<TMP_Text>(); 
            if (tmpOld != null && (tmpOld.text.ToUpper().Contains("CAJAS") || tmpOld.text.ToUpper().Contains("ESCAPE") || tmpOld.text.ToUpper().Contains("MISI"))) { Destroy(obj); continue; }

            if (obj.GetComponent<Camera>() != null || obj.GetComponent<Light>() != null || 
                obj.name.Contains("Canvas") || obj.name.Contains("Input") || obj.name.Contains("Camera")) continue;
            
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
            container.layer = 0; // Regresamos a Default para que el raycast lo detecte

            var inst = gltf.InstantiateMainSceneAsync(container.transform);
            while (!inst.IsCompleted) yield return null;

            // Limpieza de componentes internos
            Component[] componentes = container.GetComponentsInChildren<Component>(true);
            foreach (var comp in componentes)
            {
                if (comp == null || comp is Transform || comp is MeshRenderer || comp is MeshFilter || comp is SkinnedMeshRenderer) 
                    continue;
                
                string nombreTipo = comp.GetType().Name;
                if (comp is Camera || comp is AudioListener || nombreTipo.Contains("Cinemachine") || nombreTipo.Contains("Camera"))
                {
                    Destroy(comp);
                }
            }

            // Fsicas y Colisin
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
