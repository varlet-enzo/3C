using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [Header("Composants UI")]
    public Image icon;
    public Text nameText; // Remplace par TMP_Text si tu utilises TextMeshPro
    public Text amountText; // Remplace par TMP_Text si tu utilises TextMeshPro

    private InventoryItem currentItem; // On mémorise l'objet contenu dans cette case

    // Remplissage visuel de la case
    public void Setup(InventoryItem item)
    {
        currentItem = item; // On sauvegarde la donnée
        
        icon.sprite = item.itemData.icon;
        if (nameText != null) nameText.text = item.itemData.displayName;
        if (amountText != null) amountText.text = "x" + item.amount.ToString();
    }

    // Fonction appelée par le composant Button de Unity (On Click)
    public void OnClick()
    {
        // On vérifie que le joueur existe bien
        if (PlayerInventory.Instance != null && currentItem != null)
        {
            // 1. On envoie l'objet au joueur
            PlayerInventory.Instance.AddItem(currentItem.itemData, currentItem.amount);
            
            // 2. On détruit le bouton visuel du coffre (l'objet a été pris)
            Destroy(gameObject);
            
            // Note : Pour un système complet, il faudrait aussi retirer l'objet 
            // de la liste du script Chest.cs, mais on commence simple !
        }
    }
}