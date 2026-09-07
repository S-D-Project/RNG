using System;

[Serializable]
public abstract class TargetingResourceData
{
    public float SearchRange = 10f;
    public abstract ITargeting Create();
}