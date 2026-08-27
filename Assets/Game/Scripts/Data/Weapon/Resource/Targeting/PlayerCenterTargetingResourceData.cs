using System;

[Serializable]
public class PlayerCenterTargetingResourceData : TargetingResourceData
{
    public override ITargeting Create()
    {
        return new PlayerCenterTargeting();
    }
}