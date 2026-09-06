using System;

[Serializable]
public abstract class MovementResourceData
{
    public abstract IAttackMovementFactory CreateFactory();
}