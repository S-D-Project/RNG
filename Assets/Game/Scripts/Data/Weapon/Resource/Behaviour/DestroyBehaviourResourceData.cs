using System;

[Serializable]
public class DestroyBehaviourResourceData : BehaviourResourceData
{
    public override IWeaponBehaviour Create()
    {
        return new DestroyBehaviour();
    }
}