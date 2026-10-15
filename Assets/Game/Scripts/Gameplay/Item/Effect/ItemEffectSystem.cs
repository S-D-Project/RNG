using System.Collections.Generic;
using UnityEngine;

public class ItemEffectSystem
{
    private readonly WeaponStatEffectHandler
        _weaponStatHandler = new();

    public bool TryApply(
        ItemRuntime item,
        IWeaponStatEffectTarget weapon)
    {
        if (item == null || item.BaseData == null)
        {
            Debug.LogError("ItemRuntime is not initialized.");
            return false;
        }

        if (weapon == null)
        {
            Debug.LogError("Weapon target is null.");
            return false;
        }

        IReadOnlyList<ItemEffectData> effects =
            item.BaseData.Effects;

        if (effects == null || effects.Count == 0)
            return false;

        // 모든 효과가 처리 가능한지 먼저 확인
        foreach (ItemEffectData effect in effects)
        {
            if (!CanApply(effect))
            {
                Debug.LogWarning(
                    $"Unsupported item effect: {item.Id}");
                return false;
            }
        }

        // 효과 적용
        foreach (ItemEffectData effect in effects)
        {
            ApplyEffect(effect, weapon);
        }

        return true;
    }

    private bool CanApply(ItemEffectData effect)
    {
        if (effect == null)
            return false;

        switch (effect.EffectType)
        {
            case ItemEffectType.WeaponStat:
                return _weaponStatHandler.CanApply(effect);

            default:
                return false;
        }
    }

    private void ApplyEffect(
        ItemEffectData effect,
        IWeaponStatEffectTarget weapon)
    {
        switch (effect.EffectType)
        {
            case ItemEffectType.WeaponStat:
                _weaponStatHandler.Apply(effect, weapon);
                break;
        }
    }
}