using System.Collections.Generic;
using UnityEngine;

public class MetaInventory : MonoBehaviour
{
    public List<ItemData> ownedItems = new();

    public void Add(ItemData item)
    {
        if (item == null) return;
        ownedItems.Add(item);
        Debug.Log($"Meta saved item: {item.itemName}");
    }
}
