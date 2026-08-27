using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game/Data/Registry/Weapon Resource Registry")]
public class WeaponResourceRegistry : ScriptableObject
{
    [SerializeField]
    private List<WeaponResource> _weaponRegistry;

    public WeaponResource Find(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        string normalizedId = id.Trim();
        
        
        foreach (WeaponResource resource in _weaponRegistry)
        {
            if (resource == null)
            {
                continue;
            }

            if (string.Equals(resource.Id, normalizedId, StringComparison.Ordinal))
            {
                return resource;
            }
        }

        return null;
    }

}