using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class ForwardTargetingResourceData : TargetingResourceData
{
    [Range(1f,360f)]
    public float SearchAngle;
    public override bool RequiresTarget => true;
    public override ITargeting Create()
    {
        return new ForwardTargeting(SearchAngle);
    }
}