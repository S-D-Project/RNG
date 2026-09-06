using System;

[Serializable]
public class HomingMovementResourceData : MovementResourceData
{
    public float TurnSpeed;
    public float SearchInterval;

    
    public override IAttackMovement Create()
    {
        return new HomingAttackMovement(TurnSpeed, SearchInterval);
    }
}