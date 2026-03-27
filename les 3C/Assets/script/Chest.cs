using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Contenu du Coffre")]
    public List<InventoryItem> items = new List<InventoryItem>();

    [Header("Références UI (Requises pour tes Tools)")]
    public GameObject chestCanvas; // Le panneau principal
    public GameObject popupUI;     // Le texte "Appuyez sur E..."
    public Transform itemsGrid;    // L'endroit où les objets s'affichent

    [Header("Manager Modulaire")]
    public ChestUI uiManager; // Le script ChestUI qu'on a créé tout à l'heure

    private bool isOpen = false;

    // --- Fonction appelée quand le joueur fait "Action" ---
    public void OnInteract()
    {
        if (!isOpen) OpenChest();
        else CloseChest();
    }

    public void OpenChest()
    {
        isOpen = true;
        ShowPopup(false); // On cache l'infobulle quand le coffre est ouvert
        
        // On ouvre via le manager modulaire (ou directement le canvas)
        if (uiManager != null) 
            uiManager.Open(this);
        else if (chestCanvas != null)
            chestCanvas.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void CloseChest()
    {
        isOpen = false;
        
        if (uiManager != null) 
            uiManager.Close();
        else if (chestCanvas != null)
            chestCanvas.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    // --- Fonction appelée par le PlayerController quand il regarde le coffre ---
    public void ShowPopup(bool show)
    {
        if (popupUI != null)
        {
            popupUI.SetActive(show);
        }
    }
}