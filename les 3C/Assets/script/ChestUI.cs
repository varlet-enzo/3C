using UnityEngine;

public class ChestUI : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject canvasPanel; // Le panneau principal à activer
    public Transform slotContainer; // L'endroit où les cases vont apparaître (Grid Layout)
    public GameObject slotPrefab; // Le bouton Prefab à copier

    public void Open(Chest chest)
    {
        canvasPanel.SetActive(true);

        // 1. On nettoie l'interface (on supprime les anciens objets)
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. On crée une case pour chaque objet dans le coffre
        foreach (InventoryItem item in chest.items)
        {
            GameObject go = Instantiate(slotPrefab, slotContainer);
            InventorySlotUI slot = go.GetComponent<InventorySlotUI>();
            slot.Setup(item);
        }
    }

    public void Close()
    {
        canvasPanel.SetActive(false);
    }
}