using UnityEngine;

public class WeaponData
{
    public string Id { get; }

    public float Damage { get; }
    public float DamagePerLevel { get; }

    public float FireInterval { get; }
    public float FireIntervalPerLevel { get; }

    public float Range { get; }
    public float RangePerLevel { get; }

    public float Speed { get; }
    public float SpeedPerLevel { get; }

    public float HitRadius { get; }
    public float Lifetime { get; }

    public string WeaponName { get; }

    public GameObject WeaponObjectPrefab { get; }
    public Sprite Icon { get; }
    public WeaponType WeaponType { get; }

    public FirePatternResourceData FirePattern { get; }
    public FireModeResourceData FireMode { get; }
    public TargetingResourceData Targeting { get; }
    public AttackDefinitionData AttackDefinitionData { get; }

    public WeaponData(
        WeaponDto weaponDto,
        WeaponResource weaponResource)
    {
        Id = weaponDto.Id;

        Damage = weaponDto.Damage;
        DamagePerLevel = weaponDto.DamagePerLevel;

        FireInterval = weaponDto.FireInterval;
        FireIntervalPerLevel = weaponDto.FireIntervalPerLevel;

        Range = weaponDto.Range;
        RangePerLevel = weaponDto.RangePerLevel;

        Speed = weaponDto.Speed;
        SpeedPerLevel = weaponDto.SpeedPerLevel;

        HitRadius = weaponDto.HitRadius;
        Lifetime = weaponDto.Lifetime;

        WeaponName = weaponResource.WeaponName;
        WeaponObjectPrefab = weaponResource.WeaponObjectPrefab;
        Icon = weaponResource.Icon;
        WeaponType = weaponResource.WeaponType;

        FireMode = weaponResource.FireMode;
        FirePattern = weaponResource.FirePattern;
        Targeting = weaponResource.Targeting;
        AttackDefinitionData = weaponResource.AttackDefinitionData;
    }
}