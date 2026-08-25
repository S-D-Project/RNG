using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game/Data/Registry/Weapon Resource Registry")]
public class WeaponResourceRegistry : ScriptableObject
{
    [SerializeField]
    private List<WeaponResource> _weaponRegistry;

    public WeaponResource Find(string id)
    {
        foreach (WeaponResource resource in _weaponRegistry)
        {
            if (resource.Id == id)
            {
                return resource;
            }
        }

        return null;
    }

}