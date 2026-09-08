using System;

[Serializable]
public class TimeIntervalHitPolicyResourceData : HitPolicyResourceData
{
    public float HitInterval;

    public override IHitPolicy Create()
    {
        return new TimeIntervalHitPolicy(HitInterval);
    }
}