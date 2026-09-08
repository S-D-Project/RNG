using System;

[Serializable]
public class ForwardTargetingResourceData : TargetingResourceData
{
    public override bool RequiresTarget => true;
    public override ITargeting Create()
    {
        return new ForwardTargeting();
    }
}