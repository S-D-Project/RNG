using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private WeaponRuntime _weaponRuntime;
    private ISpawnPosition _spawnPosition;
    private ITargeting _targeting;
    private IFirePattern _firePattern;
    private IFireMode _fireMode;
    private IReadOnlyList<IWeaponBehaviour> _behaviours;
    private IAttackMovementFactory _movementFactory;
    
    private bool _isInitialized;
    private AttackRuntimeManager _attackRuntimeManager;

    public bool IsCooldownReady => _weaponRuntime.Cooldown <= 0f;
    public bool IsOwnerMoving { get; private set; }

    public AttackRuntimeManager AttackRuntimeManager => _attackRuntimeManager;

    public void SetOwnerMoving(bool isMoving)
    {
        IsOwnerMoving = isMoving;
    }
    public void Initialize(WeaponRuntime runtime,AttackRuntimeManager attackRuntimeManager)
    {
        if (_isInitialized)
        {
            return;
        }
        
        _weaponRuntime = runtime;
        _spawnPosition = runtime.BaseData.AttackDefinitionData.SpawnPosition.Create();
        _targeting = runtime.BaseData.Targeting.Create();
        _firePattern = runtime.BaseData.FirePattern.Create();
        _fireMode = runtime.BaseData.FireMode.Create();
        _attackRuntimeManager = attackRuntimeManager;
        _movementFactory = runtime.BaseData.AttackDefinitionData.Movement.CreateFactory();
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
        
        _weaponRuntime.UpdateCooldown(deltaTime);
        
        _fireMode.Update(this,_weaponRuntime,deltaTime);
    }

    public bool TryFindTarget(out EnemyRuntime target)
    {
        target = FindTarget();

        return target != null;
    }
    public bool TryFireNow()
    {
        if (!IsCooldownReady)
        {
            return false;
        }

        EnemyRuntime target = null;

        if (_targeting.RequiresTarget && !TryFindTarget(out target))
        {
            return false;
        }
        
        return TryFireNow(target);
    }

    public bool TryFireNow(EnemyRuntime target)
    {
        if (!IsCooldownReady)
        {
            return false;
        }

        Fire(target);
        
        _weaponRuntime.ResetCooldown();

        return true;
    }
    
    private void Fire(EnemyRuntime target)
    {
        Vector2 ownerPosition = transform.position;
        Vector2 targetPosition;
        Vector2 aimDirection;

        if (_targeting.RequiresTarget)
        {
            targetPosition = target.transform.position;
            aimDirection = (targetPosition - ownerPosition).normalized;
        }
        else
        {
            targetPosition = ownerPosition;
            aimDirection = Vector2.up;
            ;
        }

        Vector2 spawnPosition = _spawnPosition.GetPosition(ownerPosition, targetPosition, aimDirection);

        Vector2 toTarget = targetPosition - spawnPosition;

        Vector2 baseDirection = toTarget.sqrMagnitude > Mathf.Epsilon ? toTarget.normalized : aimDirection;

        IReadOnlyList<Vector2> directions = _firePattern.GetDirections(baseDirection);

        foreach (Vector2 direction in directions)
        {
            FireAttack(spawnPosition,direction,target);
        }
    }

    private void FireAttack(Vector2 spawnPosition,Vector2 direction,EnemyRuntime target)
    {
        AttackDefinitionData resource =
            _weaponRuntime.BaseData.AttackDefinitionData;
        IHitPolicy hitPolicy = resource.HitPolicy.Create();

        if (!_movementFactory.TryCreateMovement(direction, out IAttackMovement attackMovement))
        {
            return;
        }

        _attackRuntimeManager.Spawn(
            resource.AttackPrefab,
            spawnPosition,
            transform,
            target,
            direction,
            _weaponRuntime.CurrentSpeed,
            _weaponRuntime.CurrentDamage,
            _weaponRuntime.BaseData.HitRadius,
            _weaponRuntime.BaseData.Lifetime,
            attackMovement,
            hitPolicy,
            _behaviours);
    }
    
    private EnemyRuntime FindTarget()
    {
        Vector2 position = transform.position;

        return _targeting.FindTarget(position, EnemyManager.Instance.SpatialQuery);
    }

}