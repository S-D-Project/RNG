using System;

[Serializable]
public class NoneTargetingResourceData : TargetingResourceData
{
    public override bool RequiresTarget => false;

    public override ITargeting Create()
    {
        return new NoneTargeting();
    }
}