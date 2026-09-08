using System;

[Serializable]
public abstract class TargetingResourceData
{
    public float SearchRange = 10f;
    
    public abstract bool RequiresTarget { get; }
    public abstract ITargeting Create();
}