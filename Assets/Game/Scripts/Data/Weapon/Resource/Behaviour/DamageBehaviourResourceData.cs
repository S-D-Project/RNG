using System;

[Serializable]
public class DamageBehaviourResourceData : BehaviourResourceData
{
    public override IWeaponBehaviour Create()
    {
        return new DamageBehaviour();
    }
}