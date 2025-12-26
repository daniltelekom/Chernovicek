using System.Collections.Generic;
using UnityEngine;

public enum RuneRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class RuneModifier
{
    public ItemStatType statType;
    public float value;
}

[CreateAssetMenu(menuName = "Game/Rune")]
public class RuneData : ScriptableObject
{
    public string runeId;
    public string runeName;
    public RuneRarity rarity;

    [TextArea] public string description;

    public List<RuneModifier> modifiers = new();
}
