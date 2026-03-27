using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    // Le Singleton : permet d'y accéder de n'importe où via PlayerInventory.Instance
    public static PlayerInventory Instance { get; private set; }

    [Header("Contenu du Sac")]
    public List<InventoryItem> items = new List<InventoryItem>();

    void Awake()
    {
        // Initialisation du Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Sécurité pour n'avoir qu'un seul inventaire
    }

    // La fonction pour recevoir un objet
    public void AddItem(ItemData data, int amount)
    {
        // 1. On vérifie si le joueur a déjà cet objet pour l'empiler (Stack)
        foreach (InventoryItem item in items)
        {
            if (item.itemData == data)
            {
                item.amount += amount;
                Debug.Log(amount + " " + data.displayName + " ajoutés. Total : " + item.amount);
                return; // On arrête la fonction ici
            }
        }

        // 2. Si le joueur n'avait pas cet objet, on crée une nouvelle case dans son sac
        items.Add(new InventoryItem(data, amount));
        Debug.Log("Nouvel objet ramassé : " + data.displayName + " (x" + amount + ")");
    }
}