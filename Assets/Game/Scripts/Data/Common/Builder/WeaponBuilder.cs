using System.Collections.Generic;
using UnityEngine;

public class WeaponBuilder
{
    private readonly WeaponResourceRegistry _weaponRegistry;

    public WeaponBuilder(WeaponResourceRegistry weaponRegistry)
    {
        _weaponRegistry = weaponRegistry;
    }

    public List<WeaponData> Build(IReadOnlyList<WeaponDto> dtos)
    {
        var result = new List<WeaponData>();

        foreach (WeaponDto dto in dtos)
        {
            if (string.IsNullOrWhiteSpace(dto.Id))
            {
                Debug.LogWarning("WeaponDto Id is null or empty.");
                continue;
            }

            string id = dto.Id.Trim();
            WeaponResource resource = _weaponRegistry.Find(id);

            if (resource == null)
            {
                Debug.LogWarning(
                    $"WeaponResource was not found : {id}");

                continue;
            }

            result.Add(new WeaponData(dto, resource));
        }

        return result;
    }
}