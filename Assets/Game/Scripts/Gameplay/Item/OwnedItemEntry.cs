public class OwnedItemEntry
{
    public ItemRuntime Item { get; }
    public WeaponRuntime TargetWeapon { get; }

    public OwnedItemEntry(ItemRuntime item, WeaponRuntime weapon)
    {
        Item = item;
        TargetWeapon = weapon;
    }
}