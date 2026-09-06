using System;
using UnityEngine;

[Serializable]
public class OrbitMovementResourceData : MovementResourceData
{
    public AngularCenterType CenterType;
    public Vector2 CenterOffset;
    public float Radius = 1f;

    public AngularDistributionData Distribution = new();
    
    public override IAttackMovementFactory CreateFactory()
    {
        return new OrbitMovementFactory(CenterType, CenterOffset, Radius,Distribution.Create());
    }
}

public enum AngularCenterType
{
    SpawnPosition,
    Owner,
    Target
}