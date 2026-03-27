using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
    [Header("Interface Joueur")]
    public GameObject inventoryCanvas;
    public PlayerInventoryDisplay display; // NOUVEAU : Le lien vers l'afficheur

    private bool isOpen = false;

    void OnInventory() { ToggleInventory(); }

    void Start()
    {
        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
            isOpen = false;
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        
        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(isOpen);
        }

        // NOUVEAU : Si on ouvre l'inventaire, on rafraîchit les cases
        if (isOpen && display != null)
        {
            display.RefreshDisplay();
        }

        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }
}