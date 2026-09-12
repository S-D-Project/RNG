using System;

[Serializable]
public class RandomTargetingResourceData : TargetingResourceData
{
    public override bool RequiresTarget => true;
    public override ITargeting Create()
    {
        return new RandomTargeting();
    }
}