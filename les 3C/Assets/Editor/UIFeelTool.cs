using UnityEngine;
using UnityEditor;

public class UIFeelTool : EditorWindow
{
    private UIAnimationPreset currentPreset;

    // On l'ajoute au même endroit que l'outil de caméra pour faire un vrai "Package" !
    [MenuItem("Tools/3C Feel/UI Feel Manager")]
    public static void ShowWindow()
    {
        GetWindow<UIFeelTool>("UI Feel Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Créateur de Presets d'Animation UI", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Générez des animations d'apparition pour vos Canvas (Coffres, Inventaires, Menus).", MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("Créer un nouveau Preset UI", GUILayout.Height(30)))
        {
            CreateNewPreset();
        }

        GUILayout.Space(15);
        
        currentPreset = (UIAnimationPreset)EditorGUILayout.ObjectField("Modifier un Preset", currentPreset, typeof(UIAnimationPreset), false);

        if (currentPreset != null)
        {
            GUILayout.Space(10);
            Editor editor = Editor.CreateEditor(currentPreset);
            editor.OnInspectorGUI();
        }
    }

    private void CreateNewPreset()
    {
        UIAnimationPreset newPreset = ScriptableObject.CreateInstance<UIAnimationPreset>();
        string path = EditorUtility.SaveFilePanelInProject("Sauvegarder Preset UI", "NewUIPreset", "asset", "Sauvegarder le preset");
        
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