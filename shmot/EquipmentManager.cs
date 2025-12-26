using System.Collections.Generic;
using UnityEngine;

public struct EquipmentTotals
{
    public float damageMult;              // множитель (1.0 = без бонуса)
    public float cooldownMult;            // множитель (меньше = быстрее)
    public float projectileSpeedMult;     // множитель
    public float radiusAdd;               // плоская прибавка
    public float moveSpeedAdd;            // плоская прибавка (к скорости игрока)
    public float maxHealthAdd;            // плоская прибавка к max hp
}

public class EquipmentManager : MonoBehaviour
{
    // meta (постоянное) — сохраняется
    private readonly Dictionary<ItemSlot, ItemData> metaEquipped = new();

    // run (временное) — очищается после забега
    private readonly Dictionary<ItemSlot, ItemData> runEquipped = new();

    PlayerUpgrades upg;
    PlayerController controller;
    PlayerStats stats;

    void Awake()
    {
        upg = GetComponent<PlayerUpgrades>();
        controller = GetComponent<PlayerController>();
        stats = GetComponent<PlayerStats>();
    }

    // ===== Public API =====

    public ItemData GetEquipped(ItemSlot slot, bool isRun)
        => isRun ? (runEquipped.TryGetValue(slot, out var r) ? r : null)
                 : (metaEquipped.TryGetValue(slot, out var m) ? m : null);

    public void Equip(ItemData item, bool isRun)
    {
        if (item == null) return;

        var dict = isRun ? runEquipped : metaEquipped;

        // если слот занят — заменяем
        dict[item.slot] = item;

        RecalculateAndApply();
    }

    public void Unequip(ItemSlot slot, bool isRun)
    {
        var dict = isRun ? runEquipped : metaEquipped;

        if (dict.ContainsKey(slot))
            dict.Remove(slot);

        RecalculateAndApply();
    }

    /// <summary>
    /// Сбрасываем всё ран-экипирование (в конце/при выходе из рана).
    /// </summary>
    public void ClearRunEquipment()
    {
        runEquipped.Clear();
        RecalculateAndApply();
    }

    /// <summary>
    /// Эффективный предмет для слота: если в run есть — берём run, иначе meta.
    /// </summary>
    public ItemData GetEffective(ItemSlot slot)
    {
        if (runEquipped.TryGetValue(slot, out var r) && r != null) return r;
        if (metaEquipped.TryGetValue(slot, out var m) && m != null) return m;
        return null;
    }

    // ===== Core =====

    public void RecalculateAndApply()
    {
        var totals = CalculateTotals();

        // 1) отдать totals в PlayerUpgrades как "equipment layer"
        if (upg == null) upg = GetComponent<PlayerUpgrades>();
        if (upg != null) upg.SetEquipmentTotals(totals);

        // 2) скорость (не трогаем базу — кладём в bonus)
        if (controller == null) controller = GetComponent<PlayerController>();
        if (controller != null) controller.equipmentMoveSpeedBonus = totals.moveSpeedAdd;

        // 3) хп (если есть поддержка)
        if (stats == null) stats = GetComponent<PlayerStats>();
        if (stats != null) stats.SetEquipmentMaxHealthBonus(totals.maxHealthAdd);
    }

    EquipmentTotals CalculateTotals()
    {
        // базовые (без бонусов)
        EquipmentTotals t = new EquipmentTotals
        {
            damageMult = 1f,
            cooldownMult = 1f,
            projectileSpeedMult = 1f,
            radiusAdd = 0f,
            moveSpeedAdd = 0f,
            maxHealthAdd = 0f
        };
        // считаем по эффективному шмоту
        ApplyItem(GetEffective(ItemSlot.Head), ref t);
        ApplyItem(GetEffective(ItemSlot.Chest), ref t);
        ApplyItem(GetEffective(ItemSlot.Legs), ref t);
        ApplyItem(GetEffective(ItemSlot.Gloves), ref t);
        ApplyItem(GetEffective(ItemSlot.Boots), ref t);
        ApplyItem(GetEffective(ItemSlot.RingLeft), ref t);
        ApplyItem(GetEffective(ItemSlot.RingRight), ref t);

        return t;
    }

    static void ApplyItem(ItemData item, ref EquipmentTotals t)
    {
        if (item == null || item.modifiers == null) return;

        foreach (var mod in item.modifiers)
        {
            switch (mod.statType)
            {
                case ItemStatType.MaxHealth:
                    t.maxHealthAdd += mod.value;
                    break;

                case ItemStatType.MoveSpeed:
                    t.moveSpeedAdd += mod.value;
                    break;

                case ItemStatType.DamagePercent:
                    t.damageMult *= 1f + (mod.value / 100f);
                    break;

                case ItemStatType.CooldownReducePercent:
                    // reduction 10% => множитель 0.9
                    t.cooldownMult *= 1f - (mod.value / 100f);
                    break;

                case ItemStatType.RadiusAdd:
                    t.radiusAdd += mod.value;
                    break;

                case ItemStatType.ProjectileSpeedPercent:
                    t.projectileSpeedMult *= 1f + (mod.value / 100f);
                    break;
            }
        }
    }
}
