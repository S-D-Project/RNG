using System;

[Serializable]
public class HomingMovementResourceData : MovementResourceData
{
    public override IMovement Create()
    {
        return new HomingMovement();
    }
}