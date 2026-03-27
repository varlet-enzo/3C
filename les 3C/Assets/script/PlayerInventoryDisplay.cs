using UnityEngine;

public class PlayerInventoryDisplay : MonoBehaviour
{
    [Header("Configuration UI")]
    public Transform slotContainer; // La Grid où vont les objets
    public GameObject slotPrefab;   // Le Prefab du bouton

    // Cette fonction sera appelée quand le joueur ouvre son menu
    public void RefreshDisplay()
    {
        // 1. On vide la grille pour ne pas avoir de doublons
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        // Sécurité : si le joueur n'a pas de sac à dos, on arrête
        if (PlayerInventory.Instance == null) return;

        // 2. On crée une case pour chaque objet dans le sac du joueur
        foreach (InventoryItem item in PlayerInventory.Instance.items)
        {
            GameObject go = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slot = go.GetComponent<InventorySlotUI>();
            
            if (slot != null)
            {
                slot.Setup(item);
            }
        }
    }
}