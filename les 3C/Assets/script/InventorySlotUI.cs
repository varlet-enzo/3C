using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    // Définition des modes du bouton
    public enum SlotType { Chest, Player }
    
    [Header("Configuration")]
    public SlotType currentType;

    [Header("Composants UI")]
    public Image icon;
    public Text amountText; 
    public Text NameText; 

    // Données internes
    private InventoryItem currentItem;
    private Chest chestSource;

    /// <summary>
    /// Configure la case avec ses données et son comportement.
    /// </summary>
    /// <param name="item">L'objet à afficher</param>
    /// <param name="type">Vient-il d'un coffre ou du joueur ?</param>
    /// <param name="source">Le script Chest d'origine (optionnel)</param>
    public void Setup(InventoryItem item, SlotType type, Chest source = null)
    {
        currentItem = item;
        currentType = type;
        chestSource = source;

        // Mise à jour du visuel
        if (icon != null && item.itemData != null)
        {
            icon.sprite = item.itemData.icon;
        }

        if (amountText != null)
        {
            amountText.text = "x" + item.amount.ToString();
        }

        if (NameText != null)
        {
            NameText.text = item.itemData.displayName;
        }
    }

    /// <summary>
    /// Fonction appelée par le composant Button (On Click)
    /// </summary>
    public void OnClick()
    {
        if (currentItem == null) return;

        if (currentType == SlotType.Chest)
        {
            HandleChestClick();
        }
        else
        {
            HandlePlayerClick();
        }
    }

    // --- LOGIQUE COFFRE ---
    private void HandleChestClick()
    {
        // 1. On donne l'objet au joueur
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddItem(currentItem.itemData, currentItem.amount);
            Debug.Log($"📦 {currentItem.itemData.displayName} ajouté à l'inventaire joueur.");
        }

        // 2. On le retire de la LISTE RÉELLE du coffre pour qu'il ne réapparaisse pas
        if (chestSource != null)
        {
            chestSource.items.Remove(currentItem);
        }

        // 3. On détruit le bouton visuel
        Destroy(gameObject);
    }

    // --- LOGIQUE INVENTAIRE JOUEUR ---
    private void HandlePlayerClick()
    {
        Debug.Log($"✨ Utilisation de {currentItem.itemData.displayName} depuis l'inventaire.");

        // Exemple simple : on consomme 1 exemplaire
        currentItem.amount--;

        if (currentItem.amount <= 0)
        {
            // On retire l'objet de la liste du joueur s'il n'y en a plus
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.items.Remove(currentItem);
            }
            Destroy(gameObject);
        }
        else
        {
            // On met à jour le texte si l'objet reste
            if (amountText != null) amountText.text = "x" + currentItem.amount.ToString();
        }
    }
}