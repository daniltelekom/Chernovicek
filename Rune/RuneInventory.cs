using System.Collections.Generic;
using UnityEngine;

public class RuneInventory : MonoBehaviour
{
    [Header("Slots")]
    public int slotCount = 3;

    [Header("Owned Runes")] public List<RuneData> ownedRunes = new(); 

    // Вставленные руны (перманентно)
    public List<RuneData> equippedRunes = new();

    PlayerUpgrades upg;
    PlayerController controller;
    PlayerStats stats;

    void Awake()
    {
        upg = GetComponent<PlayerUpgrades>();
        controller = GetComponent<PlayerController>();
        stats = GetComponent<PlayerStats>();

        // заполняем список null'ами, чтобы было ровно slotCount
        EnsureSize();
    }

    void EnsureSize()
    {
        if (equippedRunes == null) equippedRunes = new List<RuneData>();
        while (equippedRunes.Count < slotCount) equippedRunes.Add(null);
        while (equippedRunes.Count > slotCount) equippedRunes.RemoveAt(equippedRunes.Count - 1);
    }

   public bool EquipRune(int slotIndex, RuneData rune)
{
    EnsureSize();
    if (slotIndex < 0 || slotIndex >= slotCount) return false;

    // если руна уже стоит в другом слоте — убираем оттуда (перенос)
    if (rune != null)
    {
        for (int i = 0; i < equippedRunes.Count; i++)
        {
            if (i == slotIndex) continue;
            if (equippedRunes[i] == rune)
            {
                equippedRunes[i] = null;
                break;
            }
        }
    }

    equippedRunes[slotIndex] = rune;
    RecalculateAndApply();
    return true;
}


    public void UnequipRune(int slotIndex)
    {
        EnsureSize();
        if (slotIndex < 0 || slotIndex >= slotCount) return;

        equippedRunes[slotIndex] = null;
        RecalculateAndApply();
    }

    public void RecalculateAndApply()
    {
        EnsureSize();

        float dmgMult = 1f;
        float cdMult = 1f;
        float projSpdMult = 1f;
        float radiusAdd = 0f;
        float moveSpeedAdd = 0f;
        float maxHpAdd = 0f;

        foreach (var r in equippedRunes)
        {
            if (r == null || r.modifiers == null) continue;

            foreach (var m in r.modifiers)
            {
                switch (m.statType)
                {
                    case ItemStatType.DamagePercent:
                        dmgMult *= 1f + m.value / 100f;
                        break;

                    case ItemStatType.CooldownReducePercent:
                        cdMult *= 1f - m.value / 100f;
                        break;

                    case ItemStatType.ProjectileSpeedPercent:
                        projSpdMult *= 1f + m.value / 100f;
                        break;

                    case ItemStatType.RadiusAdd:
                        radiusAdd += m.value;
                        break;

                    case ItemStatType.MoveSpeed:
                        moveSpeedAdd += m.value;
                        break;
                        case ItemStatType.MaxHealth:
                        maxHpAdd += m.value;
                        break;
                }
            }
        }

        public void AddRune(RuneData rune) { if (rune == null) return; ownedRunes.Add(rune); } 

        // Применение:
        // 1) в PlayerUpgrades — как отдельный слой рун
        if (upg != null)
            upg.SetRuneTotals(dmgMult, cdMult, projSpdMult, radiusAdd);

        // 2) скорость
        if (controller != null)
            controller.runeMoveSpeedBonus = moveSpeedAdd;

        // 3) хп
        if (stats != null)
            stats.SetRuneMaxHealthBonus(maxHpAdd);
    }
}
