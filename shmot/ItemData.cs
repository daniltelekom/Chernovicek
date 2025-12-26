using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string itemId;
    public string itemName;

    [Header("Slot & Rarity")]
    public ItemSlot slot;
    public ItemRarity rarity;

    [Header("Stats")]
    public List<ItemStatModifier> modifiers = new();

    [Header("Meta")]
    [TextArea]
    public string description;
}
