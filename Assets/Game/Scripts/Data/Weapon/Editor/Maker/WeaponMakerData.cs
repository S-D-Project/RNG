using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class WeaponMakerData
{
    private bool IsHomingMovement => MovementType == MovementType.Homing;
    private bool IsOrbitMovement => MovementType == MovementType.Orbit;
    
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
    [ShowIf(nameof(IsHomingMovement))]
    [LabelText("Turn Speed")]
    public float HomingTurnSpeed;
    
    [ShowIf(nameof(IsHomingMovement))]
    [LabelText("Search Interval")]
    public float HomingSearchInterval;
    
    // Orbit
    [ShowIf(nameof(IsOrbitMovement))]
    [LabelText("Orbit Center")]
    public OrbitCenterType OrbitCenterType;

    [ShowIf(nameof(IsOrbitMovement))]
    [LabelText("Orbit Offset")]
    public Vector2 OrbitCenterOffset;

    [ShowIf(nameof(IsOrbitMovement))]
    [LabelText("Radius")]
    public float OrbitRadius;
    
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