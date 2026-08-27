using System;

[Serializable]
public abstract class BehaviourResourceData
{
    public abstract IWeaponBehaviour Create();

}