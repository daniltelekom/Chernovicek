using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;

    public Transform runItemsContent;
    public GameObject runItemButtonPrefab;

    public TMP_Text chestInfoText;

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
        Refresh();
    }

    void Refresh()
    {
        // очистка списка
        foreach (Transform c in runItemsContent)
            Destroy(c.gameObject);

        // кнопки предметов
        foreach (var item in runInv.foundItems)
        {
            var go = Instantiate(runItemButtonPrefab, runItemsContent);
            var txt = go.GetComponentInChildren<TMP_Text>();
            txt.text = $"{item.itemName} [{item.slot}]";

            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => Deposit(item));
        }

        chestInfoText.text = $"Chest: {chest.storedItems.Count}/{chest.maxSlots}";
    }

    void Deposit(ItemData item)
    {
        if (chest.IsFull)
        {
            Debug.Log("Chest is full");
            return;
        }

        if (chest.TryAdd(item))
        {
            runInv.Remove(item);
            Refresh();
        }
    }

    public void ConfirmAndExitRun()
    {
        foreach (var item in chest.storedItems)
            meta.Add(item);

        runInv.ClearRun();
        chest.Clear();

        panel.SetActive(false);
        Debug.Log("Run ended, items saved");
    }
}
