using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR
public class ChestUITool : EditorWindow
{
    [MenuItem("Tools/3C/Create Chest UI")]
    public static void CreateChestUI()
    {
        // 1. Trouver le coffre sélectionné
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj == null)
        {
            Debug.LogError("❌ Veuillez sélectionner un objet COFFRE dans la scène avant de lancer cet outil !");
            return;
        }

        Chest chestScript = selectedObj.GetComponent<Chest>();
        if (chestScript == null)
        {
            chestScript = selectedObj.AddComponent<Chest>();
        }

        // ==================== CRÉATION DU CANVAS PRINCIPAL ====================
        GameObject mainCanvasGO = new GameObject("Chest_UI_Canvas");
        mainCanvasGO.transform.SetParent(selectedObj.transform);
        
        Canvas mainCanvas = mainCanvasGO.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = mainCanvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        mainCanvasGO.AddComponent<GraphicRaycaster>();

        // ==================== PANEL D'INVENTAIRE (Chest Canvas) ====================
        GameObject chestPanel = new GameObject("Inventory_Panel");
        chestPanel.transform.SetParent(mainCanvasGO.transform, false);
        
        Image panelImg = chestPanel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Remplacer par un sprite de texture plus tard
        
        RectTransform panelRect = chestPanel.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(800, 600);
        panelRect.anchoredPosition = Vector2.zero;

        // Titre
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(chestPanel.transform, false);
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "COFFRE";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontSize = 40;
        titleText.color = Color.white;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 250);
        titleRect.sizeDelta = new Vector2(400, 100);

        // Grid pour les items (Placeholder)
        GameObject gridGO = new GameObject("Grid_Items");
        gridGO.transform.SetParent(chestPanel.transform, false);
        GridLayoutGroup grid = gridGO.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(100, 100);
        grid.spacing = new Vector2(10, 10);
        RectTransform gridRect = gridGO.GetComponent<RectTransform>();
        gridRect.sizeDelta = new Vector2(700, 400);

        // ==================== POPUP "PRESS E" (World Space) ====================
        GameObject popupGO = new GameObject("Popup_PressE");
        popupGO.transform.SetParent(selectedObj.transform, false);
        popupGO.transform.localPosition = new Vector3(0, 2f, 0); // Un peu au dessus du coffre

        Canvas popupCanvas = popupGO.AddComponent<Canvas>();
        popupCanvas.renderMode = RenderMode.WorldSpace;
        RectTransform popupRect = popupGO.GetComponent<RectTransform>();
        popupRect.sizeDelta = new Vector2(300, 100);
        // Important pour World Space
        popupRect.localScale = new Vector3(0.01f, 0.01f, 0.01f); 

        // Billboard script (pour que ça regarde toujours la caméra)
        // (Optionnel, ou ajouter un script Billboard simple si vous en avez un)

        // Fond du popup
        Image popupBg = popupGO.AddComponent<Image>();
        popupBg.color = new Color(0, 0, 0, 0.7f);

        // Ajout du texte "Ouvrir [E]"
        GameObject popupTextGO = new GameObject("Text");
        popupTextGO.AddComponent<RectTransform>();
        popupTextGO.transform.SetParent(popupGO.transform, false);
        
        Text popupText = popupTextGO.AddComponent<Text>();
        popupText.text = "Ouvrir [E]";
        
        // CORRECTION : Charger une police par défaut fiable pour éviter les carrés roses
        popupText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if(popupText.font == null) popupText.font = Resources.FindObjectsOfTypeAll<Font>()[0];

        popupText.alignment = TextAnchor.MiddleCenter;
        popupText.resizeTextForBestFit = true;
        popupText.color = Color.yellow;
        RectTransform ptRect = popupTextGO.GetComponent<RectTransform>();
        ptRect.anchorMin = Vector2.zero;
        ptRect.anchorMax = Vector2.one;
        ptRect.offsetMin = new Vector2(10, 10);
        ptRect.offsetMax = new Vector2(-10, -10);


        // ==================== ASSIGNATION AUTOMATIQUE ====================
        chestScript.chestCanvas = chestPanel; // Le Panel (dans le ScreenOverlay)
        chestScript.popupUI = popupGO;        // Le Popup (dans le WorldSpace)
        chestScript.itemsGrid = gridGO.transform; // La Grille pour les items

        // Désactiver par défaut pour le clean setup
        chestPanel.SetActive(false);
        popupGO.SetActive(false);

        Debug.Log($"✅ UI créée et assignée sur {selectedObj.name} !");
    }
}
#endif
