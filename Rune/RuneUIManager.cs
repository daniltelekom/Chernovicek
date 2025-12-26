using System.Text;
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

    [Header("Details UI")]
    public TMP_Text detailsTitle;
    public TMP_Text detailsMods;
    public TMP_Text detailsDesc;

    [Header("Optional")]
    public TMP_Text infoText;

    int selectedSlotIndex = 0;
    RuneData selectedRuneForDetails;

    void Start()
    {
        if (runeInv == null)
            runeInv = FindObjectOfType<RuneInventory>();

        BuildSlots();
        RefreshAll();

        // по умолчанию покажем руну из выбранного слота (если есть)
        ShowDetails(runeInv != null ? runeInv.equippedRunes[selectedSlotIndex] : null);
    }

    void BuildSlots()
    {
        foreach (Transform c in slotsRoot) Destroy(c.gameObject);

        for (int i = 0; i < runeInv.slotCount; i++)
        {
            int idx = i;

            var go = Instantiate(runeSlotButtonPrefab, slotsRoot);
            var btn = go.GetComponent<Button>();

            btn.onClick.AddListener(() =>
            {
                selectedSlotIndex = idx;

                var inSlot = runeInv.equippedRunes[idx];
                ShowDetails(inSlot);

                // если слот занят — второй клик снимает (простая логика)
                // если хочешь: можно сделать "снимать отдельной кнопкой"
                if (inSlot != null)
                {
                    runeInv.UnequipRune(idx);
                    RefreshAll();
                    ShowDetails(null);
                }
                else
                {
                    RefreshSlots();
                }
            });
        }
    }

    void RefreshAll()
    {
        RefreshSlots();
        RefreshList();
        if (infoText != null) infoText.text = $"Selected Slot: {selectedSlotIndex + 1}";
    }

    void RefreshSlots()
    {
        for (int i = 0; i < slotsRoot.childCount; i++)
        {
            var go = slotsRoot.GetChild(i).gameObject;
            var label = go.GetComponentInChildren<TMP_Text>(true);

            var r = runeInv.equippedRunes[i];
            string text = r == null ? $"Slot {i + 1}: Empty" : $"Slot {i + 1}: {r.runeName}";
            if (i == selectedSlotIndex) text = "▶️ " + text;

            if (label != null) label.text = text;
        }
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
                // 1) показываем детали выбранной руны
                ShowDetails(r);

                // 2) вставляем в выбранный слот
                runeInv.EquipRune(selectedSlotIndex, r);

                RefreshAll();
            });
        }
    }
    void ShowDetails(RuneData rune)
    {
        selectedRuneForDetails = rune;

        if (detailsTitle == null || detailsMods == null || detailsDesc == null)
            return;

        if (rune == null)
        {
            detailsTitle.text = "No rune selected";
            detailsMods.text = "";
            detailsDesc.text = "";
            return;
        }

        detailsTitle.text = $"{rune.runeName} [{rune.rarity}]";

        var sb = new StringBuilder();
        if (rune.modifiers != null)
        {
            foreach (var m in rune.modifiers)
            {
                sb.AppendLine(FormatModifier(m.statType, m.value));
            }
        }
        detailsMods.text = sb.ToString();

        detailsDesc.text = string.IsNullOrWhiteSpace(rune.description) ? "" : rune.description;
    }

    string FormatModifier(ItemStatType stat, float value)
    {
        // красиво выводим
        return stat switch
        {
            ItemStatType.DamagePercent => $"+{value:0.#}% Damage",
            ItemStatType.CooldownReducePercent => $"-{value:0.#}% Cooldown",
            ItemStatType.ProjectileSpeedPercent => $"+{value:0.#}% Projectile Speed",
            ItemStatType.RadiusAdd => $"+{value:0.#} Radius",
            ItemStatType.MoveSpeed => $"+{value:0.#} Move Speed",
            ItemStatType.MaxHealth => $"+{value:0.#} Max HP",
            _ => $"+{value:0.#} {stat}"
        };
    }
}
