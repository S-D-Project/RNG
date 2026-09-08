using System;
using UnityEngine;

[Serializable]
public class FollowMovementResourceData : MovementResourceData
{
    public FollowTargetType TargetType;
    public FollowMode FollowMode;
    public float FollowSpeed;
    public Vector2 Offset;
    
    public override IAttackMovementFactory CreateFactory()
    {
        return new FollowMovementFactory(TargetType, FollowMode, FollowSpeed,Offset);
    }
}