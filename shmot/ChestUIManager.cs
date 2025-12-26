using UnityEngine;

public class ChestUIManager : MonoBehaviour
{
    public GameObject panel;

    RunInventory runInv;
    ChestStash chest;
    MetaInventory meta;

    void Awake()
    {
        panel.SetActive(false);
    }

    public void Open(PlayerStats player)
    {
        runInv = player.GetComponent<RunInventory>();
        chest = player.GetComponent<ChestStash>();
        meta = player.GetComponent<MetaInventory>();

        if (chest == null) chest = player.gameObject.AddComponent<ChestStash>();
        if (meta == null) meta = player.gameObject.AddComponent<MetaInventory>();

        panel.SetActive(true);

        Debug.Log("Chest opened");
        Debug.Log($"Run items: {runInv.foundItems.Count}");
    }

    // ВРЕМЕННАЯ КНОПКА — кладёт ПЕРВЫЙ предмет
    public void DepositFirstItem()
    {
        if (runInv.foundItems.Count == 0) return;

        var item = runInv.foundItems[0];

        if (chest.TryAdd(item))
        {
            runInv.Remove(item);
            Debug.Log($"Deposited to chest: {item.itemName}");
        }
        else
        {
            Debug.Log("Chest full!");
        }
    }

    public void ConfirmAndExitRun()
    {
        foreach (var item in chest.storedItems)
            meta.Add(item);

        // чистим ран
        runInv.ClearRun();
        chest.Clear();

        panel.SetActive(false);

        Debug.Log("Run ended. Chest items saved.");
    }
}
