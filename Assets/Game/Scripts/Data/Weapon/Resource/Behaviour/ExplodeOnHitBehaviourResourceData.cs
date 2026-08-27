using System;

[Serializable]
public class ExplodeOnHitBehaviourResourceData : BehaviourResourceData
{
    public override IWeaponBehaviour Create()
    {
        return new ExplodeOnHitWeaponBehaviour();
    }
}