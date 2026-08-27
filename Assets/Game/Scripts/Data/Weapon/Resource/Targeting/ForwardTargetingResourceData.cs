using System;

[Serializable]
public class ForwardTargetingResourceData : TargetingResourceData
{
    public override ITargeting Create()
    {
        return new ForwardTargeting();
    }
}