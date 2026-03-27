using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerInventoryGenerator : EditorWindow
{
    private GameObject playerObject;
    private GameObject slotPrefab;

    [MenuItem("Tools/3C Feel/Generateur Inventaire Joueur")]
    public static void ShowWindow()
    {
        GetWindow<PlayerInventoryGenerator>("Générateur Inventaire");
    }

    void OnGUI()
    {
        GUILayout.Label("Générateur Automatique d'Interface", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Cet outil va créer le Canvas, la Grille, et lier tous les scripts à ton joueur en un seul clic.", MessageType.Info);

        GUILayout.Space(15);

        playerObject = (GameObject)EditorGUILayout.ObjectField("1. Ton Joueur (Player)", playerObject, typeof(GameObject), true);
        
        // On réutilise le petit bouton que tu as déjà fabriqué pour le coffre !
        slotPrefab = (GameObject)EditorGUILayout.ObjectField("2. Ton Prefab de Case (Slot)", slotPrefab, typeof(GameObject), false);

        GUILayout.Space(20);

        if (GUILayout.Button("Générer l'Inventaire complet !", GUILayout.Height(40)))
        {
            if (playerObject != null && slotPrefab != null)
            {
                GenerateUI();
            }
            else
            {
                Debug.LogWarning("Il manque le joueur ou le Prefab pour générer l'UI !");
            }
        }
    }

    private void GenerateUI()
    {
        // 1. Création du Canvas principal
        GameObject canvasObj = new GameObject("PlayerInventory_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Création du Panneau de fond (Gris transparent)
        GameObject panelObj = new GameObject("Background_Panel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Gris foncé
        
        // On centre le panneau pour qu'il ne prenne pas tout l'écran
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.2f);
        panelRect.anchorMax = new Vector2(0.8f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 3. Création de la Grille de rangement
        GameObject gridObj = new GameObject("Grid_Container");
        gridObj.transform.SetParent(panelObj.transform, false);
        GridLayoutGroup grid = gridObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(100, 100); // Taille des cases
        grid.spacing = new Vector2(10, 10);    // Espace entre les cases
        
        // On étire la grille sur le panneau
        RectTransform gridRect = gridObj.GetComponent<RectTransform>();
        gridRect.anchorMin = Vector2.zero;
        gridRect.anchorMax = Vector2.one;
        gridRect.offsetMin = new Vector2(20, 20); // Marges
        gridRect.offsetMax = new Vector2(-20, -20);

        // 4. Ajout des scripts sur le Joueur (s'il ne les a pas déjà)
        PlayerInventory invData = playerObject.GetComponent<PlayerInventory>();
        if (invData == null) invData = playerObject.AddComponent<PlayerInventory>();

        PlayerInventoryUI invUI = playerObject.GetComponent<PlayerInventoryUI>();
        if (invUI == null) invUI = playerObject.AddComponent<PlayerInventoryUI>();

        PlayerInventoryDisplay invDisplay = playerObject.GetComponent<PlayerInventoryDisplay>();
        if (invDisplay == null) invDisplay = playerObject.AddComponent<PlayerInventoryDisplay>();

        // 5. On fait tous les liens magiquement
        invUI.inventoryCanvas = canvasObj;
        invUI.display = invDisplay;
        invDisplay.slotContainer = gridObj.transform;
        invDisplay.slotPrefab = slotPrefab;

        // 6. On désactive le Canvas pour qu'il soit fermé au début du jeu
        canvasObj.SetActive(false);

        // On sélectionne le nouveau Canvas pour te montrer le résultat
        Selection.activeGameObject = canvasObj;
        Debug.Log("🎉 Interface d'inventaire générée avec succès !");
    }
}