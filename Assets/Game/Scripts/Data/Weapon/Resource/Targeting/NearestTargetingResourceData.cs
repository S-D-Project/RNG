using System;

[Serializable]
public class NearestTargetingResourceData : TargetingResourceData
{
    public override ITargeting Create()
    {
        return new NearestTargeting();
    }
}