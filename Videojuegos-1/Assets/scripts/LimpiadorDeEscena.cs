using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using GLTFast;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

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
        if (SceneManager.GetActiveScene().name == "MenuMain") {
            ConfigurarMenuPrincipal();
        }

        for (int i = 0; i < 5; i++)
        {
            EjecutarLimpiezaDeTextos();
            yield return new WaitForSeconds(1f);
        }
    }

    void ConfigurarMenuPrincipal()
    {
        GameObject btnSelector = GameObject.Find("BtnSelector") ?? GameObject.Find("LevelSelector") ?? GameObject.Find("Selector");
        if (btnSelector != null) btnSelector.SetActive(false);

        TMP_FontAsset fuenteTematica = Resources.Load<TMP_FontAsset>("fonts/Smythe-Regular") ?? Resources.Load<TMP_FontAsset>("Smythe-Regular");
        
        Button[] botones = GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (Button b in botones) {
            TMP_Text t = b.GetComponentInChildren<TMP_Text>();
            if (t != null && fuenteTematica != null) {
                t.font = fuenteTematica;
                t.color = Color.white;
            }
        }
    }

    void EjecutarLimpiezaDeTextos()
    {
        GameObject[] todos = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in todos)
        {
            if (obj == null || obj.scene.name == null) continue;
            string nombre = obj.name.ToUpper();

            if (obj.GetComponent<Canvas>() != null || obj.GetComponent<Slider>() != null || 
                nombre.Contains("BARRA") || nombre.Contains("VIDA") || 
                nombre.Contains("INFO_CONTROLES") || nombre.Contains("HISTORIA_OFRENDA")) continue;

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
        tm.text = nombreEscena == "EscenarioCalavera" ? "¡CUIDADO CON LOS ENEMIGOS!\nLleva el pan a la meta." : "¡BIENVENIDO!\nUsa el portal para ir a la calavera.";
        tm.fontSize = 120;
        tm.characterSize = 0.05f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
        texto3D.transform.position = new Vector3(0, 8, 85);

        GameObject botonPortal = null;
        if (nombreEscena == "Nivel 1")
        {
            botonPortal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            botonPortal.name = "BOTON_PORTAL_CALAVERA";
            botonPortal.transform.position = new Vector3(0, 0.1f, 85); 
            botonPortal.transform.localScale = new Vector3(2, 0.1f, 2);
            botonPortal.GetComponent<Renderer>().material.color = Color.magenta;
            botonPortal.GetComponent<Collider>().isTrigger = true;
            botonPortal.AddComponent<BotonTeletransporte>();

            GameObject textoBoton = new GameObject("TextoBoton");
            textoBoton.transform.SetParent(botonPortal.transform);
            textoBoton.transform.localPosition = new Vector3(0, 8, 0); 
            TextMesh tmBoton = textoBoton.AddComponent<TextMesh>();
            tmBoton.text = "PÍSAME PARA IR\nA LA CALAVERA";
            tmBoton.fontSize = 100;
            tmBoton.characterSize = 0.05f;
            tmBoton.anchor = TextAnchor.MiddleCenter;
            tmBoton.alignment = TextAlignment.Center;
            tmBoton.color = Color.magenta;
            textoBoton.transform.localScale = new Vector3(0.5f, 5, 0.5f);
        }

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
            foreach (Transform t in jugador.GetComponentsInChildren<Transform>(true)) protegidos.Add(t.gameObject);
            
            VidaJugador vidaComp = jugador.GetComponent<VidaJugador>();
            if (vidaComp != null && vidaComp.barraDeVidaUI != null) {
                GameObject rootUI = vidaComp.barraDeVidaUI.transform.root.gameObject;
                protegidos.Add(rootUI);
                foreach (Transform t in rootUI.GetComponentsInChildren<Transform>(true)) protegidos.Add(t.gameObject);
            }

            GameObject cam = GameObject.Find("PlayerFollowCamera");
            if (cam != null) protegidos.Add(cam);
            
            var tpControl = jugador.GetComponent<StarterAssets.ThirdPersonController>();
            var controller = jugador.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;
            
            if (nombreEscena == "EscenarioCalavera")
            {
                jugador.transform.position = new Vector3(0, 30, 40);
                if (tpControl != null) { tpControl.MoveSpeed = 8.0f; tpControl.SprintSpeed = 15.0f; }
            }
            else
            {
                jugador.transform.position = new Vector3(0, 1.5f, 0);
                if (tpControl != null) { tpControl.MoveSpeed = 2.0f; tpControl.SprintSpeed = 5.33f; }
            }
            yield return null;
            if (controller != null) controller.enabled = true;
        }

        EjecutarLimpiezaDeTextos();

        GameObject[] todosActivos = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in todosActivos)
        {
            if (obj == null || protegidos.Contains(obj)) continue;
            
            if (obj.GetComponent<Canvas>() != null || obj.GetComponent<Slider>() != null) continue;

            if (obj.GetComponent<VidaEnemigo>() != null) continue;
            
            if (obj.GetComponent<Camera>() != null || obj.GetComponent<Light>() != null || 
                obj.name.Contains("Input") || obj.name.Contains("EventSystem") ||
                obj.name.Contains("MainCamera")) continue;
            Destroy(obj);
        }

        if (nombreEscena == "EscenarioCalavera")
        {
            StartCoroutine(CargarMapaCalavera(jugador));
        }
        else if (nombreEscena == "Nivel 1")
        {
            GenerarControlesTutorial();
            StartCoroutine(GenerarMuseoDiaDeMuertos());
        }
    }

    IEnumerator CargarMapaCalavera(GameObject jugador)
    {
        TextAsset glbDataCalavera = Resources.Load<TextAsset>("skull_dia_de_muertos");
        if (glbDataCalavera == null) yield break;

        var gltf = new GltfImport();
        var success = gltf.LoadGltfBinary(glbDataCalavera.bytes);
        while (!success.IsCompleted) yield return null;

        if (success.Result)
        {
            GameObject mapa = new GameObject("MAPA_CALAVERA");
            mapa.transform.position = Vector3.zero;
            mapa.transform.localScale = Vector3.one * 30f;
            var inst = gltf.InstantiateMainSceneAsync(mapa.transform);
            while (!inst.IsCompleted) yield return null;

            foreach (var cam in mapa.GetComponentsInChildren<Camera>(true)) Destroy(cam.gameObject);
            foreach (var mr in mapa.GetComponentsInChildren<MeshRenderer>(true)) mr.gameObject.AddComponent<MeshCollider>();
            
            GenerarPowerUps();
            GenerarMetaVictoria();
            
            TextAsset glbDataPan = Resources.Load<TextAsset>("pan_de_muerto");
            if (glbDataPan != null) {
                for (int i = 0; i < 4; i++) StartCoroutine(InstanciarPan(glbDataPan.bytes, i));
            }

            GenerarEnemigos(jugador);
        }
    }

    void GenerarEnemigos(GameObject jugador)
    {
        GameObject prefabSkeleton = Resources.Load<GameObject>("Skeleton"); 
        
        for (int i = 0; i < 6; i++)
        {
            GameObject enemigo;
            if (prefabSkeleton != null) {
                enemigo = Instantiate(prefabSkeleton);
                Debug.Log("Esqueleto real instanciado.");
            } else {
                Debug.LogWarning("No se encontró el prefab Skeleton en Resources. Usando cilindro.");
                enemigo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                enemigo.GetComponent<Renderer>().material.color = Color.red;
                enemigo.AddComponent<VidaEnemigo>();
                enemigo.AddComponent<SeguirJugador>();
                enemigo.AddComponent<Animator>(); 
            }

            enemigo.name = "EnemigoLento_" + i;
            enemigo.transform.position = new Vector3(Random.Range(-40, 40), -24, Random.Range(40, 80));
            enemigo.transform.localScale = Vector3.one * 1.5f; // Un poco más grandes

            if (enemigo.GetComponent<AtaqueEnemigo>() == null) {
                enemigo.AddComponent<AtaqueEnemigo>();
            }
            CapsuleCollider col = enemigo.GetComponent<CapsuleCollider>();
            if (col == null) col = enemigo.AddComponent<CapsuleCollider>();
            col.isTrigger = true;
            col.center = new Vector3(0, 1, 0);
            col.height = 2;
            
            NavMeshAgent agent = enemigo.GetComponent<NavMeshAgent>();
            if (agent == null) agent = enemigo.AddComponent<NavMeshAgent>();
            
            if (agent != null) {
                agent.speed = 1.5f;
                agent.acceleration = 4f;
                agent.stoppingDistance = 2f;
            }

            SeguirJugador seguir = enemigo.GetComponent<SeguirJugador>();
            if (seguir == null) seguir = enemigo.AddComponent<SeguirJugador>();
            if (seguir != null && jugador != null) {
                seguir.objetivo = jugador.transform;
                seguir.velocidadManual = 1.5f;
            }
        }
    }

    void GenerarPowerUps()
    {
        for (int i = 0; i < 15; i++)
        {
            GameObject pu = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            pu.name = "PowerUp_Salto_" + i;
            pu.transform.position = new Vector3(Random.Range(-50, 50), -22, Random.Range(-50, 50));
            pu.transform.localScale = Vector3.one * 0.75f; 
            pu.GetComponent<Renderer>().material.color = Color.cyan;
            GameObject luzObj = new GameObject("Luz_PU");
            luzObj.transform.SetParent(pu.transform);
            luzObj.transform.localPosition = Vector3.zero;
            Light luz = luzObj.AddComponent<Light>();
            luz.type = LightType.Point;
            luz.color = Color.cyan;
            luz.intensity = 3f;
            luz.range = 8f;
            pu.GetComponent<Collider>().isTrigger = true;
            pu.AddComponent<PowerUpSalto>();
            pu.AddComponent<GirarItem>(); 
        }
    }

    void GenerarMetaVictoria()
    {
        GameObject meta = GameObject.CreatePrimitive(PrimitiveType.Cube);
        meta.name = "PLATAFORMA_META";
        meta.transform.position = new Vector3(0, -24, 10); 
        meta.transform.localScale = new Vector3(5, 0.5f, 5);
        meta.GetComponent<Renderer>().material.color = Color.yellow;
        meta.GetComponent<Collider>().isTrigger = true;
        meta.AddComponent<MetaFinal>();
        GameObject luzMeta = new GameObject("Luz_Meta");
        luzMeta.transform.SetParent(meta.transform);
        luzMeta.transform.localPosition = new Vector3(0, 2, 0);
        Light luz = luzMeta.AddComponent<Light>();
        luz.type = LightType.Point;
        luz.color = Color.yellow;
        luz.intensity = 10f;
        luz.range = 15f;
        GameObject textoMeta = new GameObject("TextoMeta");
        textoMeta.transform.SetParent(meta.transform);
        textoMeta.transform.localPosition = new Vector3(0, 2, 0);
        TextMesh tm = textoMeta.AddComponent<TextMesh>();
        tm.text = "ENTREGA EL PAN AQUÍ";
        tm.fontSize = 24;
        tm.characterSize = 0.1f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.color = Color.yellow;
    }

    IEnumerator InstanciarPan(byte[] bytes, int index)
    {
        var gltf = new GltfImport();
        var success = gltf.LoadGltfBinary(bytes);
        while (!success.IsCompleted) yield return null;
        if (success.Result)
        {
            GameObject container = new GameObject("Pan_" + index);
            container.transform.position = new Vector3(Random.Range(-30, 30), -22, Random.Range(20, 60));
            container.transform.localScale = Vector3.one * 3f;
            container.tag = "Cargable";
            var inst = gltf.InstantiateMainSceneAsync(container.transform);
            while (!inst.IsCompleted) yield return null;
            Component[] componentes = container.GetComponentsInChildren<Component>(true);
            foreach (var comp in componentes)
            {
                if (comp == null || comp is Transform || comp is MeshRenderer || comp is MeshFilter || comp is SkinnedMeshRenderer) continue;
                if (comp is Camera || comp is AudioListener || comp.GetType().Name.Contains("Cinemachine")) Destroy(comp);
            }
            Rigidbody rb = container.AddComponent<Rigidbody>();
            rb.mass = 2f;

            GameObject luzPan = new GameObject("Luz_Pan");
            luzPan.transform.SetParent(container.transform);
            luzPan.transform.localPosition = new Vector3(0, 0.5f, 0);
            Light luz = luzPan.AddComponent<Light>();
            luz.type = LightType.Point;
            luz.color = new Color(1.0f, 0.6f, 0.2f);
            luz.intensity = 5f;
            luz.range = 8f;

            MeshRenderer mr = container.GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                BoxCollider bc = container.AddComponent<BoxCollider>();
                bc.center = container.transform.InverseTransformPoint(mr.bounds.center);
                bc.size = mr.bounds.size;
                bc.isTrigger = false;
            } else container.AddComponent<SphereCollider>();
        }
    }

    void GenerarControlesTutorial()
    {
        GameObject info = new GameObject("Controles_Tutorial");
        info.name = "INFO_CONTROLES_FIJO";
        info.transform.position = new Vector3(-10, 3, 5);
        TextMesh tm = info.AddComponent<TextMesh>();
        tm.text = "CONTROLES:\nWASD - MOVERSE\nESPACIO - SALTAR\nE - AGARRAR/SOLTAR PAN\nESC - PAUSA";
        tm.fontSize = 120;
        tm.characterSize = 0.05f;
        tm.color = Color.yellow;
        tm.anchor = TextAnchor.UpperLeft;
    }

    IEnumerator GenerarMuseoDiaDeMuertos()
    {
        string[] modelos = { "altar", "flor", "craneo", "catrin", "papel_picado" };
        string[] descripciones = {
            "EL ALTAR: Ofrenda para honrar a los difuntos.",
            "FLOR CEMPASÚCHIL: Guía las almas con su aroma.",
            "CALAVERA DE AZÚCAR: Aceptación de la muerte.",
            "EL CATRÍN: La muerte nos iguala a todos.",
            "PAPEL PICADO: Representa el aire y la alegría."
        };

        for (int i = 0; i < modelos.Length; i++)
        {
            TextAsset data = Resources.Load<TextAsset>(modelos[i]);
            if (data == null) continue;

            var gltf = new GltfImport();
            var success = gltf.LoadGltfBinary(data.bytes);
            while (!success.IsCompleted) yield return null;

            if (success.Result)
            {
                GameObject expo = new GameObject("Exposicion_" + modelos[i]);
                expo.transform.position = new Vector3(10, 0, 5 + (i * 15)); 
                
                GameObject modeloObj = new GameObject("Modelo");
                modeloObj.transform.SetParent(expo.transform);
                
                float escalaBase = (modelos[i] == "altar" ? 0.5f : 1.5f);
                float offset_y = 1f;
                if (modelos[i] == "flor") { escalaBase = 7.5f; offset_y = -9f; }
                else if (modelos[i] == "catrin") offset_y = -0.5f; 
                else if (modelos[i] == "papel_picado") offset_y = 0.5f; 

                modeloObj.transform.localPosition = Vector3.up * offset_y;
                modeloObj.transform.localScale = Vector3.one * escalaBase;
                
                var inst = gltf.InstantiateMainSceneAsync(modeloObj.transform);
                while (!inst.IsCompleted) yield return null;
                
                if (modelos[i] != "altar") modeloObj.AddComponent<GirarItem>();

                GameObject texto = new GameObject("Descripcion");
                texto.transform.SetParent(expo.transform);
                texto.transform.localPosition = new Vector3(0, 3, 0);
                TextMesh tm = texto.AddComponent<TextMesh>();
                tm.text = descripciones[i];
                tm.fontSize = 100;
                tm.characterSize = 0.05f;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.alignment = TextAlignment.Center;
                tm.color = Color.cyan;
            }
        }

        GameObject historia = new GameObject("HISTORIA_OFRENDA_TEXTO");
        historia.transform.position = new Vector3(0, 4, 75);
        TextMesh tmH = historia.AddComponent<TextMesh>();
        tmH.text = "EL PAN DE MUERTO ES EL ALIMENTO DEL ALMA.\n" +
                   "En el siguiente nivel, deberás recoger el pan\n" +
                   "y colocarlo con respeto en el CENTRO DE LA OFRENDA.\n" +
                   "Solo así el ciclo se completará y alcanzarás la victoria.";
        tmH.fontSize = 90;
        tmH.characterSize = 0.04f;
        tmH.color = Color.white;
        tmH.anchor = TextAnchor.MiddleCenter;
        tmH.alignment = TextAlignment.Center;
    }
}
