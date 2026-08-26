using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

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
    [LabelText("Icon")]
    [SerializeField]
    private Sprite _icon;

    [TitleGroup("Common")]
    [LabelText("Weapon Type")]
    [EnumToggleButtons]
    [SerializeField]
    private WeaponType _weaponType;

    // WeaponBehaviour
    [TitleGroup("Weapon Behaviour")]
    [LabelText("Fire Pattern")]
    [SerializeField]
    private FirePatternType _firePatternType;

    [TitleGroup("Weapon Behaviour")]
    [LabelText("Targeting")]
    [SerializeField]
    private TargetingType _targetingType;

    //Type Settings

    [TitleGroup("Type Settings")]
    [ShowIf("@_weaponType == WeaponType.Projectile")]
    [HideLabel]
    [SerializeField]
    private ProjectileWeaponMakerData _projectile = new();

    [TitleGroup("Type Settings")]
    [ShowIf("@_weaponType == WeaponType.Melee")]
    [HideLabel]
    [SerializeField]
    private MeleeWeaponMakerData _melee = new();

    [TitleGroup("Type Settings")]
    [ShowIf("@_weaponType == WeaponType.Area")]
    [HideLabel]
    [SerializeField]
    private AreaWeaponMakerData _area = new();

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
            _icon,
            _weaponType,
            CreateFirePattern(),
            CreateTargeting(),
            CreateTypeResource());

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

        if (_weaponType == WeaponType.Projectile && _projectile.ProjectilePrefab == null)
        {
            return false;
        }

        return true;
    }

    private FirePatternResourceData CreateFirePattern()
    {
        return _firePatternType switch
        {
            FirePatternType.Single => new SingleFirePatternResourceData(),
            FirePatternType.Burst => new BurstFirePatternResourceData(),
            FirePatternType.Spread => new SpreadFirePatternResourceData(),
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

    private ProjectileMovementResourceData CreateProjectileMovement()
    {
        return _projectile.MovementType switch
        {
            ProjectileMovementType.Straight => new StraightMovementResourceData(),
            ProjectileMovementType.Homing => new HomingMovementResourceData(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private ProjectileWeaponResourceData CreateProjectileResource()
    {
        var resource = new ProjectileWeaponResourceData();

        resource.Initialize(
            _projectile.ProjectilePrefab,
            CreateProjectileMovement(),
            CreateProjectileBehaviours());

        return resource;
    }

    private WeaponTypeResourceData CreateTypeResource()
    {
        return _weaponType switch
        {
            WeaponType.Projectile => CreateProjectileResource(),
            WeaponType.Melee => throw new NotImplementedException(),
            WeaponType.Area => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private ProjectileBehaviourResourceData CreateProjectileBehaviour(ProjectileBehaviourType behaviourType)
    {
        return behaviourType switch
        {
            ProjectileBehaviourType.Pierce => new PierceBehaviourResourceData(),
            ProjectileBehaviourType.ExplodeOnHit => new ExplodeOnHitBehaviourResourceData(),
            ProjectileBehaviourType.DestroyOnHit => new DestroyOnHitBehaviourResourceData(),
            _ => throw new ArgumentOutOfRangeException(nameof(behaviourType), behaviourType, null)
        };
    }

    private List<ProjectileBehaviourResourceData> CreateProjectileBehaviours()
    {
        return _projectile.Behaviours.Select(CreateProjectileBehaviour).ToList();
    }
}