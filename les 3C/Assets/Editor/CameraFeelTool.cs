using UnityEngine;
using UnityEditor;

public class CameraFeelTool : EditorWindow
{
    private CameraShakePreset currentPreset;

    [MenuItem("Tools/3C Feel/Camera Feel Manager")]
    public static void ShowWindow()
    {
        GetWindow<CameraFeelTool>("Camera Feel Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Créateur de Presets d'Animation d'Écran", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Ce tool permet aux Game Designers de créer et paramétrer les effets de caméra (Juiciness) sans coder.", MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("Créer un nouveau Preset de Shake", GUILayout.Height(30)))
        {
            CreateNewPreset();
        }

        GUILayout.Space(15);
        
        currentPreset = (CameraShakePreset)EditorGUILayout.ObjectField("Modifier un Preset", currentPreset, typeof(CameraShakePreset), false);

        if (currentPreset != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Paramètres de l'animation :", EditorStyles.boldLabel);
            
            Editor editor = Editor.CreateEditor(currentPreset);
            editor.OnInspectorGUI();
        }
    }

    private void CreateNewPreset()
    {
        CameraShakePreset newPreset = ScriptableObject.CreateInstance<CameraShakePreset>();
        
        string path = EditorUtility.SaveFilePanelInProject("Sauvegarder Preset", "NewShake", "asset", "Sauvegarder le preset");
        
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(newPreset, path);
            AssetDatabase.SaveAssets();
            
            currentPreset = newPreset;
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newPreset;
        }
    }
}