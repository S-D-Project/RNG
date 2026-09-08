using System;
using UnityEngine;

[Serializable]
public class ExpandingOrbitMovementResourceData : MovementResourceData
{
    public AngularCenterType CenterType;
    public Vector2 CenterOffset;
    public float InitialRadius = 1f;
    public float RadialSpeed = 1f;
    
    public override IAttackMovementFactory CreateFactory()
    {
        return new ExpandingOrbitMovementFactory(CenterType, CenterOffset, InitialRadius, RadialSpeed);
    }
}