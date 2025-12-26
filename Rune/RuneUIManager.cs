using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuneUIManager : MonoBehaviour
{
    [Header("Refs")]
    public RuneInventory runeInv;

    [Header("Slots UI")]
    public Transform slotsRoot;
    public GameObject runeSlotButtonPrefab;

    [Header("List UI")]
    public Transform listContent;
    public GameObject runeListButtonPrefab;

    [Header("Optional")]
    public TMP_Text infoText;

    int selectedSlotIndex = 0;

    void Start()
    {
        if (runeInv == null)
            runeInv = FindObjectOfType<RuneInventory>();

        BuildSlots();
        RefreshAll();
    }

    void BuildSlots()
    {
        // очистим
        foreach (Transform c in slotsRoot) Destroy(c.gameObject);

        // создаём кнопки слотов
        for (int i = 0; i < runeInv.slotCount; i++)
        {
            int idx = i;

            var go = Instantiate(runeSlotButtonPrefab, slotsRoot);
            var btn = go.GetComponent<Button>();
            var label = go.GetComponentInChildren<TMP_Text>(true);

            btn.onClick.AddListener(() =>
            {
                selectedSlotIndex = idx;

                // если слот занят — снимаем
                if (runeInv.equippedRunes[idx] != null)
                {
                    runeInv.UnequipRune(idx);
                    RefreshAll();
                }
                else
                {
                    // пустой слот просто выбираем
                    RefreshSlots();
                }
            });

            // подпись слота (обновим в RefreshSlots)
        }
    }

    void RefreshAll()
    {
        RefreshSlots();
        RefreshList();
    }

    void RefreshSlots()
    {
        for (int i = 0; i < slotsRoot.childCount; i++)
        {
            var go = slotsRoot.GetChild(i).gameObject;
            var label = go.GetComponentInChildren<TMP_Text>(true);

            var r = runeInv.equippedRunes[i];
            string text = r == null ? $"Slot {i + 1}: Empty" : $"Slot {i + 1}: {r.runeName}";

            // подсветим выбранный слот
            if (i == selectedSlotIndex) text = "▶️ " + text;

            if (label != null) label.text = text;
        }

        if (infoText != null)
            infoText.text = $"Selected Slot: {selectedSlotIndex + 1}";
    }

    void RefreshList()
    {
        foreach (Transform c in listContent) Destroy(c.gameObject);

        foreach (var r in runeInv.ownedRunes)
        {
            if (r == null) continue;

            var go = Instantiate(runeListButtonPrefab, listContent);
            var btn = go.GetComponent<Button>();
            var label = go.GetComponentInChildren<TMP_Text>(true);

            if (label != null)
                label.text = $"{r.runeName} [{r.rarity}]";
                btn.onClick.AddListener(() =>
            {
                // вставляем в выбранный слот
                runeInv.EquipRune(selectedSlotIndex, r);
                RefreshAll();
            });
        }
    }
}
