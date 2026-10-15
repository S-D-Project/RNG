public class WeaponStatEffectHandler
{
    private const string MultiplierId = "Multiplier";

    public bool CanApply(ItemEffectData effect)
    {
        if (effect == null)
            return false;

        if (effect.EffectType != ItemEffectType.WeaponStat)
            return false;

        if (effect.ReferenceId != MultiplierId)
            return false;

        return effect.Value > 0f &&
               !float.IsNaN(effect.Value) &&
               !float.IsInfinity(effect.Value);
    }

    public void Apply(
        ItemEffectData effect,
        IWeaponStatEffectTarget weapon)
    {
        weapon.MultiplyDamage(effect.Value);
    }
}