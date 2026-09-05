using System;
using UnityEngine;

[Serializable]
public class OrbitMovementResourceData : MovementResourceData
{
    public OrbitCenterType CenterType;
    public Vector2 CenterOffset;
    public float Radius = 1f;
    
    public override IMovement Create()
    {
        return new OrbitMovement(CenterType,CenterOffset,Radius);
    }
}

public enum OrbitCenterType
{
    SpawnPosition,
    Owner,
    Target
}