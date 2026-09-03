using System;

[Serializable]
public class PierceBehaviourResourceData : BehaviourResourceData
{
    public int PierceCount;
    public override IWeaponBehaviour Create()
    {
        return new PierceWeaponBehaviour(PierceCount);
    }
    
    
}