using System;

[Serializable]
public abstract class TargetingResourceData
{
    public abstract ITargeting Create();
}