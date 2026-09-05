using System;
using Sirenix.OdinInspector;

[Serializable]
public class BehaviourMakerData
{
    [LabelText("Behaviour")]
    public BehaviourType Type;
    
    // Pierce
    [ShowIf("Type",BehaviourType.Pierce)]
    [LabelText("Pierce Count")]
    public int PierceCount;
    
    // Explode
    [ShowIf("Type",BehaviourType.Explode)]
    [LabelText("Explosion Radius Multiplier")]
    public float ExplosionRadiusMultiplier;

    [ShowIf("Type",BehaviourType.Explode)]
    [LabelText("Explosion Damage Multiplier")]
    public float ExplosionDamageMultiplier = 1f;
    
}