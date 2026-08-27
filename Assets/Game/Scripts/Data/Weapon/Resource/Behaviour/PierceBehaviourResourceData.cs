using System;

[Serializable]
public class PierceBehaviourResourceData : BehaviourResourceData
{
    public override IWeaponBehaviour Create()
    {
        return new PierceWeaponBehaviour();
    }
}