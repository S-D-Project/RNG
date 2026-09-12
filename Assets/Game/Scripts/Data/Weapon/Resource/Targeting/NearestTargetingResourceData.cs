using System;

[Serializable]
public class NearestTargetingResourceData : TargetingResourceData
{
    public override bool RequiresTarget => true;
    public override ITargeting Create()
    {
        return new NearestTargeting();
    }
}