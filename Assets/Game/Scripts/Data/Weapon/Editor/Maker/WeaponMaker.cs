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
    [TitleGroup("Common")]
    [LabelText("Id")]
    [SerializeField]
    [Required]
    private string _weaponId;

    [TitleGroup("Common")]
    [LabelText("Name")]
    [SerializeField]
    [Required]
    private string _weaponName;
    
    [TitleGroup("Common")]
    [PreviewField(70)]
    [LabelText("Weapon Object Prefab")]
    [SerializeField]
    private GameObject _weaponObjectPrefab;

    [TitleGroup("Common")]
    [PreviewField(70)]
    [LabelText("Icon")]
    [SerializeField]
    private Sprite _icon;

    [TitleGroup("Common")]
    [LabelText("Weapon Type")]
    [EnumToggleButtons]
    [SerializeField]
    private WeaponType _weaponType;

    // WeaponBehaviour
    [TitleGroup("Weapon Firing")]
    [LabelText("Fire Mode")]
    [SerializeField]
    private FireModeType _fireModeType;

    [TitleGroup("Weapon Firing")]
    [LabelText("Fire Pattern")]
    [SerializeField]
    private FirePatternType _firePatternType;
    

    [TitleGroup("Weapon Firing")]
    [LabelText("Targeting")]
    [SerializeField]
    private TargetingType _targetingType;
    
    [TitleGroup("Attack Settings")]
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

        return true;
    }

    private FireModeResourceData CreateFireMode()
    {
        return _fireModeType switch
        {
            FireModeType.Instant => new InstantFireModeResourceData(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private FirePatternResourceData CreateFirePattern()
    {
        return _firePatternType switch
        {
            FirePatternType.Fan => new FanFirePatternResourceData(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private TargetingResourceData CreateTargeting()
    {
        return _targetingType switch
        {
            TargetingType.Forward => new ForwardTargetingResourceData(),
            TargetingType.Nearest => new NearestTargetingResourceData(),
            TargetingType.Random => new RandomTargetingResourceData(),
            TargetingType.PlayerCenter => new PlayerCenterTargetingResourceData(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private MovementResourceData CreateAttackMovement()
    {
        return _weapon.MovementType switch
        {
            MovementType.Straight => new StraightMovementResourceData(),
            MovementType.Homing => new HomingMovementResourceData(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private AttackDefinitionData CreateAttackDefinitionData()
    {
        var resource = new AttackDefinitionData();

        resource.Initialize(
            _weapon.AttackPrefab,
            CreateAttackMovement(),
            CreateAttackBehaviours());

        return resource;
    }
    
    private BehaviourResourceData CreateAttackBehaviour(BehaviourType behaviourType)
    {
        return behaviourType switch
        {
            BehaviourType.Pierce => new PierceBehaviourResourceData(),
            BehaviourType.ExplodeOnHit => new ExplodeOnHitBehaviourResourceData(),
            BehaviourType.DestroyOnHit => new DestroyOnHitBehaviourResourceData(),
            _ => throw new ArgumentOutOfRangeException(nameof(behaviourType), behaviourType, null)
        };
    }

    private List<BehaviourResourceData> CreateAttackBehaviours()
    {
        return _weapon.Behaviours.Select(CreateAttackBehaviour).ToList();
    }
}