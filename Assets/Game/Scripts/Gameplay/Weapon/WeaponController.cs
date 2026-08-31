using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private WeaponRuntime _weaponRuntime;
    private ITargeting _targeting;
    private IFirePattern _firePattern;
    private IReadOnlyList<IWeaponBehaviour> _behaviours;

    private float _remainingCooldown;
    private bool _isInitialized;
    private AttackRuntimeManager _attackRuntimeManager;

    public AttackRuntimeManager AttackRuntimeManager => _attackRuntimeManager;

    public void Initialize(WeaponRuntime runtime,AttackRuntimeManager attackRuntimeManager)
    {
        if (_isInitialized)
        {
            return;
        }
        
        _weaponRuntime = runtime;
        _targeting = runtime.BaseData.Targeting.Create();
        _firePattern = runtime.BaseData.FirePattern.Create();
        _attackRuntimeManager = attackRuntimeManager;

        _remainingCooldown = 0f;
        _isInitialized = true;
        _behaviours = CreateBehaviours(runtime.BaseData.WeaponResourceData);
    }

    private IReadOnlyList<IWeaponBehaviour> CreateBehaviours(WeaponResourceData resource)
    {
        List<IWeaponBehaviour> behaviours = new List<IWeaponBehaviour>();

        foreach (BehaviourResourceData behaviourResourceData in resource.Behaviours)
        {
            behaviours.Add(behaviourResourceData.Create());
        }

        return behaviours;
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }
        UpdateCooldown();
        
        
        if (_remainingCooldown > 0f)
        {
            return;
        }
        
        TryFire();
        
    }

    private void UpdateCooldown()
    {
        if (_remainingCooldown <= 0f)
        {
            return;
        }
        _remainingCooldown -= Time.deltaTime;
    }

    private void TryFire()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        EnemyRuntime target = _targeting.FindTarget(position, EnemyManager.Instance.EnemyList);

        if (target == null)
        {
            return;
        }
        
        _firePattern.Fire(this,_weaponRuntime,target);

        _remainingCooldown = _weaponRuntime.CurrentFireInterval;
    }

    public void FireAttack(Vector2 direction)
    {
        WeaponResourceData resource = _weaponRuntime.BaseData.WeaponResourceData;

        if (resource.AttackPrefab == null)
        {
            Debug.LogError("AttackPrefab is null");
            return;
        }

        IMovement movement = resource.Movement.Create();
        
        _attackRuntimeManager.Spawn(
            resource.AttackPrefab,
            transform.position,
            direction,
            _weaponRuntime.CurrentSpeed,
            _weaponRuntime.CurrentDamage,
            _weaponRuntime.BaseData.HitRadius,
            _weaponRuntime.BaseData.Lifetime,
            movement,
            _behaviours);
    }
}