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
        // Aplicar configuracin de noche en cada escena que se cargue
        AplicarNoche();
        
        // Suscribirse al evento de carga de escena para reaplicar si es necesario
        SceneManager.sceneLoaded += (scene, mode) => AplicarNoche();
    }

    static void AplicarNoche()
    {
        // 1. Buscar la Luz Direccional (el "Sol")
        Light[] luces = GameObject.FindObjectsOfType<Light>();
        foreach (Light luz in luces)
        {
            if (luz.type == LightType.Directional)
            {
                luz.intensity = 0.1f; // Mucha menos luz
                luz.color = new Color(0.1f, 0.15f, 0.3f); // Tono azul oscuro nocturno
            }
        }

        // 2. Ajustar niebla y ambiente
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.02f, 0.02f, 0.05f);
        RenderSettings.fogDensity = 0.02f;
        RenderSettings.ambientIntensity = 0.2f;
        RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.1f);

        // 3. Intentar cargar el material de cielo nocturno (si existe en la ruta)
        // Usamos el material que encontramos en la investigacin
        Material skyboxNoche = Resources.Load<Material>("FS013_Night"); 
        // Nota: Resources.Load solo funciona si est en carpeta Resources. 
        // Como no lo est, intentaremos buscarlo de otra forma o simplemente ajustar el color.
        
        // Si no podemos cargar el material por ruta exacta sin ser Editor, 
        // ajustamos el Tint del skybox actual si es posible
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
        // Esto asegura que al dar Play en el editor, SIEMPRE empiece desde el Men Principal
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
