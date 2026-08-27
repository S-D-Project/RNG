using System;

[Serializable]
public class RandomTargetingResourceData : TargetingResourceData
{
    public override ITargeting Create()
    {
        return new RandomTargeting();
    }
}