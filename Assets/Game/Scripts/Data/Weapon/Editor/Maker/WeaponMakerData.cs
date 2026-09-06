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
    [Range(0f, 1f)]
    [ShowIf(nameof(SpawnPositionType), SpawnPositionType.Between)]
    public float SpawnPositionRatio = 0.5f;

    [LabelText("Spawn Offset")]
    public Vector2 SpawnOffset;

    [LabelText("Movement")]
    public MovementType MovementType;

    // Homing
    [ShowIf("MovementType", global::MovementType.Homing)]
    [LabelText("Turn Speed")]
    public float HomingTurnSpeed;

    [ShowIf("MovementType", global::MovementType.Homing)]
    [LabelText("Search Interval")]
    public float HomingSearchInterval;

    // Orbit
    [ShowIf("MovementType", global::MovementType.Orbit)]
    [LabelText("Orbit Center")]
    public AngularCenterType OrbitCenterType;

    [ShowIf("MovementType", global::MovementType.Orbit)]
    [LabelText("Orbit Center Offset")]
    public Vector2 OrbitCenterOffset;

    [ShowIf("MovementType", global::MovementType.Orbit)]
    [LabelText("Radius")]
    public float OrbitRadius;

    [ShowIf("MovementType", global::MovementType.Orbit)]
    [LabelText("Distribution")]
    public AngularDistributionType OrbitDistributionType;

    // ExpandingOrbit

    [ShowIf("MovementType", global::MovementType.ExpandingOrbit)]
    [LabelText("Spiral Center")]
    public AngularCenterType ExpandingOrbitCenterType;

    [ShowIf("MovementType", global::MovementType.ExpandingOrbit)]
    [LabelText("Spiral Center Offset")]
    public Vector2 ExpandingOrbitCenterOffset;

    [ShowIf("MovementType", global::MovementType.ExpandingOrbit)]
    [LabelText("Initial Radius")]
    public float ExpandingOrbitInitialRadius = 1f;

    [ShowIf("MovementType", global::MovementType.ExpandingOrbit)]
    [LabelText("Radial Speed")]
    public float ExpandingOrbitRadialSpeed = 1f;


    [ShowIf("OrbitDistributionType", global::AngularDistributionType.Sequential)]
    [LabelText("Max Count")]
    [InfoBox("360도를 MaxCount로 나누어서 배치")]
    [MinValue(1)]
    public int OrbitMaxCount = 8;


    // Spiral
    [ShowIf("MovementType", global::MovementType.Spiral)]
    [LabelText("Spiral Radius")]
    public float SpiralRadius;

    [ShowIf("MovementType", global::MovementType.Spiral)]
    [LabelText("Spiral Roation Speed")]
    public float SpiralRotationSpeed;

    // Wave

    [ShowIf("MovementType", global::MovementType.Wave)]
    [LabelText("Wave Amplitude")]
    [MinValue(0f)]
    public float WaveAmplitude = 1f;

    [ShowIf("MovementType", global::MovementType.Wave)]
    [LabelText("Wave Frequency")]
    [MinValue(0f)]
    public float WaveFrequency = 1f;


    [LabelText("Behaviours")]
    [InfoBox(
        "Add behaviours applied to this projectile.",
        InfoMessageType.None)]
    [ValidateInput(
        nameof(ValidateBehaviours),
        "Duplicate behaviours are not allowed.")]
    [ListDrawerSettings(DefaultExpandedState = true, ShowFoldout = false, DraggableItems = true)]
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