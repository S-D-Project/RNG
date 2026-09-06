using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class WeaponMakerData
{
    [LabelText("Weapon Attack Prefab")]
    public GameObject AttackPrefab;
    
    [LabelText("Spawn Position")]
    public SpawnPositionType SpawnPositionType;

    [LabelText("Between Ratio")]
    [Range(0f,1f)]
    [ShowIf(nameof(SpawnPositionType), SpawnPositionType.Between)]
    public float SpawnPositionRatio = 0.5f;
    
    [LabelText("Spawn Offset")]
    public Vector2 SpawnOffset;
    
    [LabelText("Movement")]
    public MovementType MovementType;
    
    // Homing
    [ShowIf("MovementType",global::MovementType.Homing)]
    [LabelText("Turn Speed")]
    public float HomingTurnSpeed;
    
    [ShowIf("MovementType",global::MovementType.Homing)]
    [LabelText("Search Interval")]
    public float HomingSearchInterval;
    
    // Orbit
    [ShowIf("MovementType",global::MovementType.Orbit)]
    [LabelText("Orbit Center")]
    public OrbitCenterType OrbitCenterType;

    [ShowIf("MovementType",global::MovementType.Orbit)]
    [LabelText("Orbit Offset")]
    public Vector2 OrbitCenterOffset;

    [ShowIf("MovementType",global::MovementType.Orbit)]
    [LabelText("Radius")]
    public float OrbitRadius;

    [ShowIf("MovementType",global::MovementType.Orbit)]
    [LabelText("Distribution")]
    public AngularDistributionType AngularDistributionType;

    [ShowIf("AngularDistributionType", global::AngularDistributionType.Sequential)]
    [LabelText("Max Count")]
    [InfoBox("360도를 MaxCount로 나누어서 배치")]
    [MinValue(1)]
    public int OrbitMaxCount = 8;
    
    [LabelText("Behaviours")]
    [InfoBox(
        "Add behaviours applied to this projectile.",
        InfoMessageType.None)]
    [ValidateInput(
        nameof(ValidateBehaviours),
        "Duplicate behaviours are not allowed.")]
    [ListDrawerSettings(DefaultExpandedState = true,ShowFoldout = false,DraggableItems = true)]
    public List<BehaviourMakerData> Behaviours = new();

    private bool ValidateBehaviours(List<BehaviourMakerData> behaviours)
    {
        if (behaviours == null)
        {
            return true;
        }

        return behaviours.Select(x => x.Type).Distinct().Count() == behaviours.Count;
    }
}