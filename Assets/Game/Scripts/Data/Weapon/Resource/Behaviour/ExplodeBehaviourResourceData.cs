using System;

[Serializable]
public class ExplodeBehaviourResourceData : BehaviourResourceData
{
    public override IWeaponBehaviour Create()
    {
        return new ExplodeBehaviour();
    }
}