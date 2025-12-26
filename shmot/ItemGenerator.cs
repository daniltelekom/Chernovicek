using System.Collections.Generic;
using UnityEngine;

public enum DropSource
{
    Normal,
    Elite,
    MiniBoss
}

public class ItemGenerator : MonoBehaviour
{
    public static ItemData Generate(DropSource source)
    {
        var item = ScriptableObject.CreateInstance<ItemData>();

        // 1. слот
        item.slot = RandomSlot();

        // 2. редкость
        item.rarity = RollRarity(source);

        // 3. имя (временно)
        item.itemName = $"{item.rarity} {item.slot}";

        // 4. моды
        int mods = ModsByRarity(item.rarity);
        var pool = AllowedStatsForSlot(item.slot);

        for (int i = 0; i < mods; i++)
        {
            var stat = pool[Random.Range(0, pool.Count)];
            pool.Remove(stat); // без дубликатов

            item.modifiers.Add(new ItemStatModifier
            {
                statType = stat,
                value = RollStatValue(stat, item.rarity)
            });
        }

        return item;
    }

    // ===== helpers =====

    static ItemSlot RandomSlot()
    {
        var values = System.Enum.GetValues(typeof(ItemSlot));
        return (ItemSlot)values.GetValue(Random.Range(0, values.Length));
    }

    static ItemRarity RollRarity(DropSource source)
    {
        float roll = Random.value;

        if (source == DropSource.MiniBoss)
            return ItemRarity.Rare;

        if (source == DropSource.Elite)
        {
            if (roll < 0.15f) return ItemRarity.Epic;
            if (roll < 0.45f) return ItemRarity.Rare;
            if (roll < 0.75f) return ItemRarity.Uncommon;
            return ItemRarity.Common;
        }

        // Normal
        if (roll < 0.05f) return ItemRarity.Epic;
        if (roll < 0.15f) return ItemRarity.Rare;
        if (roll < 0.40f) return ItemRarity.Uncommon;
        return ItemRarity.Common;
    }

    static int ModsByRarity(ItemRarity r) => r switch
    {
        ItemRarity.Common => 1,
        ItemRarity.Uncommon => 2,
        ItemRarity.Rare => 3,
        ItemRarity.Epic => 4,
        _ => 1
    };

    static List<ItemStatType> AllowedStatsForSlot(ItemSlot slot)
    {
        return slot switch
        {
            ItemSlot.Head => new() {
                ItemStatType.MaxHealth,
                ItemStatType.CooldownReducePercent,
                ItemStatType.RadiusAdd
            },

            ItemSlot.Chest => new() {
                ItemStatType.MaxHealth,
                ItemStatType.DamagePercent
            },

            ItemSlot.Legs => new() {
                ItemStatType.MoveSpeed,
                ItemStatType.CooldownReducePercent
            },

            ItemSlot.Gloves => new() {
                ItemStatType.DamagePercent,
                ItemStatType.ProjectileSpeedPercent
            },
            ItemSlot.Boots => new() {
                ItemStatType.MoveSpeed
            },

            ItemSlot.RingLeft or ItemSlot.RingRight => new() {
                ItemStatType.DamagePercent,
                ItemStatType.RadiusAdd,
                ItemStatType.ProjectileSpeedPercent
            },

            _ => new()
        };
    }

    static float RollStatValue(ItemStatType stat, ItemRarity rarity)
    {
        return stat switch
        {
            ItemStatType.DamagePercent => rarity switch
            {
                ItemRarity.Common => Random.Range(5f, 8f),
                ItemRarity.Uncommon => Random.Range(8f, 12f),
                ItemRarity.Rare => Random.Range(12f, 18f),
                ItemRarity.Epic => Random.Range(18f, 25f),
                _ => 5f
            },

            ItemStatType.CooldownReducePercent => rarity switch
            {
                ItemRarity.Common => Random.Range(3f, 5f),
                ItemRarity.Rare => Random.Range(8f, 12f),
                ItemRarity.Epic => Random.Range(12f, 18f),
                _ => 3f
            },

            ItemStatType.MoveSpeed => rarity switch
            {
                ItemRarity.Common => Random.Range(0.2f, 0.3f),
                ItemRarity.Rare => Random.Range(0.4f, 0.6f),
                ItemRarity.Epic => Random.Range(0.6f, 0.9f),
                _ => 0.2f
            },

            ItemStatType.RadiusAdd => rarity switch
            {
                ItemRarity.Common => 0.3f,
                ItemRarity.Rare => 0.6f,
                ItemRarity.Epic => 1.0f,
                _ => 0.3f
            },

            ItemStatType.ProjectileSpeedPercent => rarity switch
            {
                ItemRarity.Common => Random.Range(8f, 12f),
                ItemRarity.Rare => Random.Range(15f, 25f),
                ItemRarity.Epic => Random.Range(25f, 40f),
                _ => 8f
            },

            ItemStatType.MaxHealth => rarity switch
            {
                ItemRarity.Common => Random.Range(10f, 20f),
                ItemRarity.Rare => Random.Range(30f, 50f),
                ItemRarity.Epic => Random.Range(60f, 100f),
                _ => 10f
            },

            _ => 1f
        };
    }
}
