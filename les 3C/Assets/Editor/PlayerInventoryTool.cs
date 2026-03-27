using UnityEngine;
using UnityEditor;

public class PlayerInventoryTool : EditorWindow
{
    private PlayerInventoryUI inventoryUI;
    private PlayerInventoryDisplay inventoryDisplay;
    
    private GameObject canvasPanel;
    private Transform slotContainer;
    private GameObject slotPrefab;

    [MenuItem("Tools/3C Feel/Player Inventory Setup")]
    public static void ShowWindow()
    {
        GetWindow<PlayerInventoryTool>("Player Inventory Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Configuration de l'UI d'Inventaire Joueur", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Assigne les éléments UI pour lier automatiquement l'inventaire du joueur à son Canvas.", MessageType.Info);

        GUILayout.Space(10);

        // On demande au GD de glisser le script PlayerInventoryUI
        inventoryUI = (PlayerInventoryUI)EditorGUILayout.ObjectField("Script PlayerInventoryUI", inventoryUI, typeof(PlayerInventoryUI), true);
        
        if (inventoryUI != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Glisse tes éléments d'interface ici :", EditorStyles.boldLabel);
            
            canvasPanel = (GameObject)EditorGUILayout.ObjectField("Canvas Panel (L'écran)", canvasPanel, typeof(GameObject), true);
            slotContainer = (Transform)EditorGUILayout.ObjectField("Slot Container (La Grille)", slotContainer, typeof(Transform), true);
            slotPrefab = (GameObject)EditorGUILayout.ObjectField("Slot Prefab (Le Bouton)", slotPrefab, typeof(GameObject), false);

            GUILayout.Space(15);

            if (GUILayout.Button("Lier l'Inventaire !", GUILayout.Height(30)))
            {
                SetupInventoryUI();
            }
        }
    }

    private void SetupInventoryUI()
    {
        // On cherche ou on ajoute le composant Display sur le même objet
        inventoryDisplay = inventoryUI.GetComponent<PlayerInventoryDisplay>();
        if (inventoryDisplay == null)
        {
            inventoryDisplay = inventoryUI.gameObject.AddComponent<PlayerInventoryDisplay>();
        }

        // On fait tous les liens magiquement
        inventoryUI.inventoryCanvas = canvasPanel;
        inventoryUI.display = inventoryDisplay;
        
        inventoryDisplay.slotContainer = slotContainer;
        inventoryDisplay.slotPrefab = slotPrefab;

        // On dit à Unity de sauvegarder ces modifications
        EditorUtility.SetDirty(inventoryUI);
        EditorUtility.SetDirty(inventoryDisplay);
        
        Debug.Log("Inventaire du joueur configuré avec succès !");
    }
}