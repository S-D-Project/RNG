using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponMaker : OdinEditorWindow
{
    // Common
    [Title("Common")]
    [LabelText("Id")]
    [SerializeField]
    [Required]
    private string _weaponId;

    [LabelText("Name")]
    [SerializeField]
    [Required]
    private string _weaponName;

    [PreviewField(70)]
    [LabelText("Weapon Object Prefab")]
    [SerializeField]
    private GameObject _weaponObjectPrefab;

    [PreviewField(70)]
    [LabelText("Icon")]
    [SerializeField]
    private Sprite _icon;

    [LabelText("Weapon Type")]
    [EnumToggleButtons]
    [SerializeField]
    private WeaponType _weaponType;

    [LabelText("Weapon Range")]
    [MinValue(1f)]
    [SerializeField]
    private float _searchRagne = 10f;

    // WeaponBehaviour
    [Title("Weapon Firing")]
    [LabelText("Fire Mode")]
    [SerializeField]
    private FireModeType _fireModeType;

    [BoxGroup("Fire Pattern")]
    [SerializeField]
    private FirePatternType _firePatternType;

    [BoxGroup("Fire Pattern")]
    [ShowIf("_firePatternType", FirePatternType.Fan)]
    [LabelText("Attack Count")]
    [SerializeField]
    private int _attackCount;

    [BoxGroup("Fire Pattern")]
    [ShowIf("_firePatternType", FirePatternType.Fan)]
    [LabelText("Spread Angle")]
    [SerializeField]
    private float _spreadAngle;

    [LabelText("Targeting")]
    [SerializeField]
    private TargetingType _targetingType;

    [Title("Attack Settings")]
    [HideLabel]
    [SerializeField]
    private WeaponMakerData _weapon = new();

    [MenuItem("Tools/Weapon Maker")]
    public static void Open()
    {
        GetWindow<WeaponMaker>("Weapon Maker");
    }

    [Button("Create Weapon", ButtonSizes.Large)]
    [GUIColor(0.4f, 0.8f, 0.4f)]
    private void CreateWeapon()
    {
        if (!Validate())
        {
            Debug.LogError("WeaponMaker validation failed");
            return;
        }

        var weaponResource = CreateInstance<WeaponResource>();

        weaponResource.Initialize(
            _weaponId,
            _weaponName,
            _weaponObjectPrefab,
            _icon,
            _weaponType,
            CreateFireMode(),
            CreateFirePattern(),
            CreateTargeting(),
            CreateAttackDefinitionData());

        var path = $"Assets/Game/SO/Weapon/{_weaponId}.asset";

        AssetDatabase.CreateAsset(weaponResource, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = weaponResource;
    }

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(_weaponId) ||
            string.IsNullOrWhiteSpace(_weaponName))
        {
            return false;
        }

        if (_weaponType == WeaponType.Projectile && _weapon.AttackPrefab == null)
        {
            return false;
        }

        if (_weapon.MovementType == MovementType.Orbit)
        {
            if (_weapon.OrbitRadius <= 0f)
            {
                return false;
            }

            if (_weapon.OrbitDistributionType ==
                AngularDistributionType.Sequential &&
                _weapon.OrbitMaxCount <= 0)
            {
                return false;
            }
        }

        return true;
    }

    private FireModeResourceData CreateFireMode()
    {
        return _fireModeType switch
        {
            FireModeType.Instant => new InstantFireModeResourceData(),
            FireModeType.Casting => new CastingFireModeResourceData(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private FirePatternResourceData CreateFirePattern()
    {
        return _firePatternType switch
        {
            FirePatternType.Fan => new FanFirePatternResourceData
            {
                AttackCount = _attackCount,
                SpreadAngle = _spreadAngle
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private TargetingResourceData CreateTargeting()
    {
        return _targetingType switch
        {
            TargetingType.Forward => new ForwardTargetingResourceData
            {
                SearchRange =  _searchRagne
            },
            TargetingType.Nearest => new NearestTargetingResourceData{
                SearchRange =  _searchRagne
            },
            TargetingType.Random => new RandomTargetingResourceData{
                SearchRange =  _searchRagne
            },
            TargetingType.PlayerCenter => new PlayerCenterTargetingResourceData{
                SearchRange =  _searchRagne
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private MovementResourceData CreateAttackMovement()
    {
        return _weapon.MovementType switch
        {
            MovementType.Straight => new StraightMovementResourceData(),
            MovementType.Homing => new HomingMovementResourceData
            {
                TurnSpeed = _weapon.HomingTurnSpeed,
                SearchInterval = _weapon.HomingSearchInterval
            },
            MovementType.Orbit => new OrbitMovementResourceData
            {
                CenterType = _weapon.OrbitCenterType,
                CenterOffset = _weapon.OrbitCenterOffset,
                Radius = _weapon.OrbitRadius,

                Distribution = new AngularDistributionData
                {
                    Type = _weapon.OrbitDistributionType,
                    MaxCount = _weapon.OrbitMaxCount
                }
            },
            MovementType.ExpandingOrbit => new ExpandingOrbitMovementResourceData
            {
                CenterType = _weapon.ExpandingOrbitCenterType,
                CenterOffset = _weapon.ExpandingOrbitCenterOffset,
                InitialRadius = _weapon.ExpandingOrbitInitialRadius,
                RadialSpeed = _weapon.ExpandingOrbitRadialSpeed
            },
            MovementType.Spiral => new SpiralMovementResourceData
            {
                Radius = _weapon.SpiralRadius,
                RotationSpeed = _weapon.SpiralRotationSpeed
            },
            MovementType.Wave => new WaveMovementResourceData
            {
                Amplitude = _weapon.WaveAmplitude,
                Frequency = _weapon.WaveFrequency
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private AttackDefinitionData CreateAttackDefinitionData()
    {
        var resource = new AttackDefinitionData();

        resource.Initialize(
            _weapon.AttackPrefab,
            CreateSpawnPosition(),
            CreateAttackMovement(),
            CreateAttackBehaviours());

        return resource;
    }

    private BehaviourResourceData CreateAttackBehaviour(BehaviourMakerData behaviour)
    {
        return behaviour.Type switch
        {
            BehaviourType.Pierce => new PierceBehaviourResourceData
            {
                PierceCount = behaviour.PierceCount
            },
            BehaviourType.Explode => new ExplodeBehaviourResourceData
            {
                ExplosionRadiusMultiplier = behaviour.ExplosionRadiusMultiplier,
                DamageMultiplier = behaviour.ExplosionDamageMultiplier
            },
            BehaviourType.Destroy => new DestroyBehaviourResourceData(),
            BehaviourType.Damage => new DamageBehaviourResourceData(),
            _ => throw new ArgumentOutOfRangeException(nameof(behaviour.Type), behaviour.Type, null)
        };
    }

    private List<BehaviourResourceData> CreateAttackBehaviours()
    {
        return _weapon.Behaviours.Select(CreateAttackBehaviour).ToList();
    }

    private SpawnPositionResourceData CreateSpawnPosition()
    {
        SpawnPositionResourceData resource = _weapon.SpawnPositionType switch
        {
            SpawnPositionType.Owner => new OwnerSpawnPositionResourceData(),
            SpawnPositionType.Target => new TargetSpawnPositionResourceData(),
            SpawnPositionType.Between => new BetweenSpawnPositionResourceData(),
            _ => throw new ArgumentOutOfRangeException()
        };

        resource.Offset = _weapon.SpawnOffset;

        return resource;
    }
}