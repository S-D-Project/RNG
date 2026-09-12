using System;

[Serializable]
public class OncePerTargetHitPolicyResourceData : HitPolicyResourceData
{
    public override IHitPolicy Create()
    {
        return new OncePerTargetHitPolicy();
    }
}