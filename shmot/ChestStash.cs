using System.Collections.Generic;
using UnityEngine;

public class ChestStash : MonoBehaviour
{
    [Header("Chest")]
    public int maxSlots = 2;

    public List<ItemData> storedItems = new();

    public bool IsFull => storedItems.Count >= maxSlots;

    public bool TryAdd(ItemData item)
    {
        if (item == null) return false;
        if (IsFull) return false;

        storedItems.Add(item);
        return true;
    }

    public void Clear()
    {
        storedItems.Clear();
    }
}
