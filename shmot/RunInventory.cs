using System.Collections.Generic;
using UnityEngine;

public class RunInventory : MonoBehaviour
{
    [Header("Run Items (temporary)")]
    public List<ItemData> foundItems = new();

    EquipmentManager equip;

    void Awake()
    {
        equip = GetComponent<EquipmentManager>();
        if (equip == null) equip = gameObject.AddComponent<EquipmentManager>();
    }

    /// <summary>
    /// Добавить предмет в забеговый инвентарь.
    /// </summary>
    public void Add(ItemData item)
    {
        if (item == null) return;
        foundItems.Add(item);
    }

    /// <summary>
    /// Экипировать предмет как run-экипировку (временная).
    /// </summary>
    public void EquipRun(ItemData item)
    {
        if (item == null) return;

        // если предмета ещё нет в списке — добавим
        if (!foundItems.Contains(item))
            foundItems.Add(item);

        equip.Equip(item, isRun: true);
    }

    /// <summary>
    /// Удалить предмет из run-инвентаря (например, выбросить).
    /// Если он был надет в run-слоте — снимем.
    /// </summary>
    public void Remove(ItemData item)
    {
        if (item == null) return;

        // если предмет сейчас надет как run — снимем его
        var eq = equip.GetEquipped(item.slot, isRun: true);
        if (eq == item)
            equip.Unequip(item.slot, isRun: true);

        foundItems.Remove(item);
    }

    public void ClearRun()
    {
        foundItems.Clear();
        equip.ClearRunEquipment();
    }
}
