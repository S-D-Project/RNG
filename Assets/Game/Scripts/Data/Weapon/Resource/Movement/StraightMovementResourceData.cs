using System;

[Serializable]
public class StraightMovementResourceData : MovementResourceData
{
    public override IMovement Create()
    {
        return new StraightMovement();
    }
}