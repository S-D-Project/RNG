using System;

[Serializable]
public abstract class HitPolicyResourceData
{
    public abstract IHitPolicy Create();
}