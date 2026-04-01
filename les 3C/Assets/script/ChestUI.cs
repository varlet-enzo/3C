using UnityEngine;
using System.Collections.Generic;

public class ChestUI : MonoBehaviour
{
    // Instance statique pour y accéder facilement depuis le script Chest.cs
    public static ChestUI Instance;

    [Header("Configuration UI")]
    public GameObject slotPrefab;   // Ton Prefab de bouton d'inventaire
    public Transform container;     // Le dossier (Content) dans ton UI
    public GameObject uiPanel;      // Le panneau parent à afficher/masquer

    // Variable CRUCIALE : On stocke ici le coffre que l'on est en train de piller
    private Chest currentChest;

    private void Awake()
    {
        // Système de Singleton pour appeler ChestUI.Instance.OpenChest()
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // On cache l'UI au démarrage
        if (uiPanel != null) uiPanel.SetActive(false);
    }

    /// <summary>
    /// Appelé par le script Chest du monde pour ouvrir cette fenêtre.
    /// </summary>
    public void OpenChest(Chest chestData)
    {
        currentChest = chestData; // On enregistre le coffre envoyé en paramètre
        
        if (uiPanel != null) uiPanel.SetActive(true);
        
        // On débloque la souris pour pouvoir cliquer
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshUI();
    }

    public void CloseChest()
    {
        if (uiPanel != null) uiPanel.SetActive(false);

        currentChest = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void RefreshUI()
    {
        if (currentChest == null) return;

        // 1. On vide l'affichage actuel (pour ne pas doubler les items)
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // 2. On crée un bouton pour chaque objet dans la liste du coffre
        foreach (InventoryItem item in currentChest.items)
        {
            GameObject newSlot = Instantiate(slotPrefab, container);
            InventorySlotUI slotScript = newSlot.GetComponent<InventorySlotUI>();

            if (slotScript != null)
            {
                // ON PASSE : L'item, le Mode Coffre, et LE COFFRE ACTUEL
                slotScript.Setup(item, InventorySlotUI.SlotType.Chest, currentChest);
            }
        }
    }
}