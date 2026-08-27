using System;

[Serializable]
public abstract class MovementResourceData
{
    public abstract IMovement Create();
}