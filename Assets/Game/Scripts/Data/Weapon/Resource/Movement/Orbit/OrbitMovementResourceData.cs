using System;
using UnityEngine;

[Serializable]
public class OrbitMovementResourceData : MovementResourceData
{
    public OrbitCenterType CenterType;
    public Vector2 CenterOffset;
    public float Radius = 1f;

    public OrbitDistributionData Distribution = new();
    
    public override IAttackMovementFactory CreateFactory()
    {
        return new OrbitMovementFactory(CenterType, CenterOffset, Radius,Distribution.Create());
    }
}

public enum OrbitCenterType
{
    SpawnPosition,
    Owner,
    Target
}