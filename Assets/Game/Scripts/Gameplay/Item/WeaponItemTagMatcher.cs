using UnityEngine;

public static class WeaponItemTagMatcher
{
    public static bool Matches(WeaponData weapon, ItemData item)
    {
        if (weapon == null || item == null)
        {
            return false;
        }
        
        WeaponTag weaponTags = weapon.WeaponTags;
        WeaponTag requiredTags = item.RequiredWeaponTags;
        
        Debug.Log($"Weapon의 WeaponTag : {weaponTags}");
        Debug.Log($"Item의 RequiredWeaponTag : {requiredTags}");

        return (weaponTags & requiredTags) == requiredTags;
    }
}