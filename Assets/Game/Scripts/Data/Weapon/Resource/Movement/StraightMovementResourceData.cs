using System;

[Serializable]
public class StraightMovementResourceData : MovementResourceData
{
    public override IAttackMovement Create()
    {
        return new StraightAttackMovement();
    }
}