
using System;

[Serializable]
public class GlobalTickHitPolicyResourceData : HitPolicyResourceData
{
    public float TickInterval;
    
    public override IHitPolicy Create()
    {
        return new GlobalTickHitPolicy(TickInterval);
    }
}