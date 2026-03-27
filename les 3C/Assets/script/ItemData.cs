using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Informations de base")]
    public string id;
    public string displayName;
    
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;

    [Header("Paramètres en jeu")]
    public bool isConsumable;
    public int maxStack = 99;
    public GameObject prefab;
}