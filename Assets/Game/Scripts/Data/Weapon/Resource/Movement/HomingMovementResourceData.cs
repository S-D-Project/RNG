using System;

[Serializable]
public class HomingMovementResourceData : MovementResourceData
{
    public float TurnSpeed;
    public float SearchInterval;
    
    public override IMovement Create()
    {
        return new HomingMovement(TurnSpeed, SearchInterval);
    }
}