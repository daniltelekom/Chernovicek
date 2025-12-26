Открой PlayerUpgrades.cs и добавь/вставь вот это (главное — смысл, можно внизу файла):

using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    // ===== RUN (карты/пассивки на забег) =====
    public float runDamageMult = 1f;
    public float runCooldownMult = 1f;
    public float runProjectileSpeedMult = 1f;
    public float runRadiusAdd = 0f;

    // ===== EQUIPMENT (шмот) =====
    public float eqDamageMult = 1f;
    public float eqCooldownMult = 1f;
    public float eqProjectileSpeedMult = 1f;
    public float eqRadiusAdd = 0f;

    // ===== Combined (то, что уже используют скиллы) =====
    public float damageMult => runDamageMult * eqDamageMult;
    public float cooldownMult => runCooldownMult * eqCooldownMult;
    public float projectileSpeedMult => runProjectileSpeedMult * eqProjectileSpeedMult;
    public float radiusAdd => runRadiusAdd + eqRadiusAdd;

    public void SetEquipmentTotals(EquipmentTotals t)
    {
        eqDamageMult = t.damageMult;
        eqCooldownMult = Mathf.Max(0.2f, t.cooldownMult); // защита от нуля
        eqProjectileSpeedMult = t.projectileSpeedMult;
        eqRadiusAdd = t.radiusAdd;
    }

    // ===== методы, которые ты уже вызываешь из пассивок =====
    public void AddDamagePercent(float percent) => runDamageMult *= 1f + percent / 100f;
    public void AddCooldownPercent(float reducePercent) => runCooldownMult *= 1f - reducePercent / 100f;
    public void AddRadius(float add) => runRadiusAdd += add;
    public void AddProjectileSpeedPercent(float percent) => runProjectileSpeedMult *= 1f + percent / 100f;

    // Твои tag-мультипликаторы (если есть) остаются как были.
    // Их можно умножать поверх combined, как ты уже делаешь.
}

Если у тебя уже есть PlayerUpgrades с tag-словарями — не удаляй их. Просто добавь “equipment layer” и combined свойства.
