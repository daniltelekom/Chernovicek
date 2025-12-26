using UnityEngine;

public class LootPickup : MonoBehaviour
{
    public ItemData item;

    [Header("Auto pickup")]
    public float pickupRadius = 1.3f;
    public bool autoEquipOnPickup = false;

    void Update()
    {
        // ищем игрока рядом (самый простой вариант)
        var p = FindObjectOfType<PlayerStats>();
        if (p == null) return;

        float d = Vector3.Distance(transform.position, p.transform.position);
        if (d > pickupRadius) return;

        var inv = p.GetComponent<RunInventory>();
        if (inv == null) inv = p.gameObject.AddComponent<RunInventory>();

        if (item != null)
        {
            inv.Add(item);
            if (autoEquipOnPickup)
                inv.EquipRun(item);

            Debug.Log($"Picked up item: {item.itemName} [{item.slot}]");
        }

        Destroy(gameObject);
    }
}
