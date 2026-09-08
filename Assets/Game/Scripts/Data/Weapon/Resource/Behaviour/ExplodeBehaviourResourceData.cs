using System;

[Serializable]
public class ExplodeBehaviourResourceData : BehaviourResourceData
{
    public float ExplosionRadiusMultiplier;
    public float DamageMultiplier;
    public override IWeaponBehaviour Create()
    {
        return new ExplodeBehaviour(ExplosionRadiusMultiplier,DamageMultiplier);
    }
}