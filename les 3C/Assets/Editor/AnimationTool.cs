using UnityEngine;
using UnityEditor;

public class AnimationTool : EditorWindow
{
    private AnimationMapping currentMapping;

    [MenuItem("Tools/3C Feel/Animation Manager")]
    public static void ShowWindow() => GetWindow<AnimationTool>("Anim Manager");

    void OnGUI()
    {
        GUILayout.Label("Configuration des Animations", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Créer un nouveau Mapping", GUILayout.Height(30)))
        {
            CreateMapping();
        }

        GUILayout.Space(10);
        currentMapping = (AnimationMapping)EditorGUILayout.ObjectField("Mapping Actuel", currentMapping, typeof(AnimationMapping), false);

        if (currentMapping != null)
        {
            Editor editor = Editor.CreateEditor(currentMapping);
            editor.OnInspectorGUI();
        }
    }

    void CreateMapping()
    {
        AnimationMapping newMapping = ScriptableObject.CreateInstance<AnimationMapping>();
        string path = EditorUtility.SaveFilePanelInProject("Sauver le Mapping", "PlayerAnimMapping", "asset", "Message");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(newMapping, path);
            AssetDatabase.SaveAssets();
            currentMapping = newMapping;
        }
    }
}