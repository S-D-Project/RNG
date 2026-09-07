using System;

[Serializable]
public class HomingMovementResourceData : MovementResourceData
{
    public float TurnSpeed = 360;
    public float SearchInterval = 2f;
    public float SearchRadius = 5f;
    
    public override IAttackMovementFactory CreateFactory()
    {
        return new HomingMovementFactory(TurnSpeed, SearchInterval,SearchRadius);
    }
}