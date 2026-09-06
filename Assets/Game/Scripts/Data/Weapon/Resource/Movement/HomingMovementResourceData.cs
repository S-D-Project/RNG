using System;

[Serializable]
public class HomingMovementResourceData : MovementResourceData
{
    public float TurnSpeed;
    public float SearchInterval;
    

    public override IAttackMovementFactory CreateFactory()
    {
        return new HomingMovementFactory(TurnSpeed, SearchInterval);
    }
}