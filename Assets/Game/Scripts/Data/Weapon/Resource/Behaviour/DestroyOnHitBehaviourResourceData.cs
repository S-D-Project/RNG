using System;

[Serializable]
public class DestroyOnHitBehaviourResourceData : BehaviourResourceData
{
    public override IWeaponBehaviour Create()
    {
        return new DestroyOnHitWeaponBehaviour();
    }
}