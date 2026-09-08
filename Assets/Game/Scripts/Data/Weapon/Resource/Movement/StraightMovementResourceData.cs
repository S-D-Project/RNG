using System;

[Serializable]
public class StraightMovementResourceData : MovementResourceData
{
    public override IAttackMovementFactory CreateFactory()
    {
        return new StraightMovementFactory();
    }
}