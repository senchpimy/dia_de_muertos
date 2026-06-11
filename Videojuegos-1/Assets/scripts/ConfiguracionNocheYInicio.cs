using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class ConfiguracionNocheYInicio : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InicializarCarga()
    {
        CrearLimpiador();
        
        AplicarNoche();
        
        SceneManager.sceneLoaded += (scene, mode) => {
            CrearLimpiador();
            AplicarNoche();
        };
    }

    static void CrearLimpiador()
    {
        if (GameObject.FindAnyObjectByType<LimpiadorDeEscena>() == null)
        {
            GameObject go = new GameObject("Configuracion_Limpiador");
            go.AddComponent<LimpiadorDeEscena>();
            GameObject.DontDestroyOnLoad(go);
        }
    }

    static void AplicarNoche()
    {
        Light[] luces = GameObject.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light luz in luces)
        {
            if (luz.type == LightType.Directional)
            {
                luz.intensity = 0.1f;
                luz.color = new Color(0.1f, 0.15f, 0.3f);
            }
        }

        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.02f, 0.02f, 0.05f);
        RenderSettings.fogDensity = 0.02f;
        RenderSettings.ambientIntensity = 0.2f;
        RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.1f);

        Material skyboxNoche = Resources.Load<Material>("FS013_Night"); 
        
        if (RenderSettings.skybox != null)
        {
            RenderSettings.skybox.SetColor("_Tint", new Color(0.1f, 0.1f, 0.2f));
        }
    }
}

#if UNITY_EDITOR
[InitializeOnLoad]
public class AutoConfiguradorInicio
{
    static AutoConfiguradorInicio()
    {
        string rutaEscenaInicio = "Assets/Scenes/MenuMain.unity";
        SceneAsset escena = AssetDatabase.LoadAssetAtPath<SceneAsset>(rutaEscenaInicio);
        
        if (escena != null)
        {
            EditorSceneManager.playModeStartScene = escena;
            Debug.Log("<color=green>Configuracin: El juego ahora siempre iniciar desde el Men Principal.</color>");
        }
    }
}
#endif
