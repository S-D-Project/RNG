using System;

[Serializable]
public abstract class TargetingResourceData
{
    public abstract bool RequiresTarget { get; }
    public abstract ITargeting Create();
}