using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private WeaponRuntime _weaponRuntime;
    private ITargeting _targeting;
    private IFirePattern _firePattern;
    private IFireMode _fireMode;
    private IReadOnlyList<IWeaponBehaviour> _behaviours;

    private float _remainingCooldown;
    private bool _isInitialized;
    private AttackRuntimeManager _attackRuntimeManager;

    public bool IsCooldownReady => _remainingCooldown <= 0f;

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
        _fireMode = runtime.BaseData.FireMode.Create();
        _attackRuntimeManager = attackRuntimeManager;

        _remainingCooldown = 0f;
        _isInitialized = true;
        _behaviours = CreateBehaviours(runtime.BaseData.AttackDefinitionData);
    }

    private IReadOnlyList<IWeaponBehaviour> CreateBehaviours(AttackDefinitionData resource)
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

        float deltaTime = Time.deltaTime;
        
        UpdateCooldown(deltaTime);
        
        _fireMode.Update(this,_weaponRuntime,deltaTime);
        
    }

    private void UpdateCooldown(float deltaTime)
    {
        if (_remainingCooldown <= 0f)
        {
            return;
        }
        _remainingCooldown -= deltaTime;
    }
    
        
    public bool TryFireNow()
    {
        EnemyRuntime target = FindTarget();

        if (target == null)
        {
            return false;
        }
        
        Fire(target);

        _remainingCooldown = _weaponRuntime.CurrentFireInterval;
        
        return true;
    }
    
    public void Fire(EnemyRuntime target)
    {
        Vector2 origin = transform.position;
        Vector2 targetPosition = target.transform.position;

        Vector2 baseDirection = (targetPosition - origin).normalized;

        IReadOnlyList<Vector2> directions = _firePattern.GetDirections(baseDirection);

        foreach (Vector2 direction in directions)
        {
            FireAttack(direction);
        }
    }

    public void FireAttack(Vector2 direction)
    {
        AttackDefinitionData resource =
            _weaponRuntime.BaseData.AttackDefinitionData;
        
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
    
    public EnemyRuntime FindTarget()
    {
        Vector2 position = transform.position;

        return _targeting.FindTarget(position, EnemyManager.Instance.EnemyList);
    }

}