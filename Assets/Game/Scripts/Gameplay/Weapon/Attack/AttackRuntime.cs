using System.Collections.Generic;
using UnityEngine;

public class AttackRuntime
{
    private readonly HashSet<EnemyRuntime> _hitTargets;
    
    public GameObject Prefab { get; }
    public GameObject Instance { get; }

    public Transform Transform => Instance.transform;
    public Transform Owner { get; }
    public EnemyRuntime Target { get; private set; }

    public Vector2 Direction { get; private set;}
    public float Speed { get; }

    public float Damage { get; }
    public float HitRadius { get; }

    public float RemainingLifetime { get; set; }
    
    public int HitCount { get; private set; }

    public IAttackMovement AttackMovement { get; }
    public IReadOnlyList<IWeaponBehaviour> Behaviours { get; }

    public bool IsDead { get; private set; }
    
    public IEnemySpatialQuery EnemySpatialQuery { get; }

    public AttackRuntime(
        GameObject prefab,
        GameObject instance,
        Transform owner,
        EnemyRuntime target,
        Vector2 direction,
        float speed,
        float damage,
        float hitRadius,
        float lifetime,
        IAttackMovement attackMovement,
        IReadOnlyList<IWeaponBehaviour> behaviours,
        IEnemySpatialQuery enemySpatialQuery)
    {
        Prefab = prefab;
        Instance = instance;
        Owner = owner;
        Target = target;

        Direction = direction.normalized;
        Speed = speed;

        Damage = damage;
        HitRadius = hitRadius;

        RemainingLifetime = lifetime;
        AttackMovement = attackMovement;
        Behaviours = behaviours;

        EnemySpatialQuery = enemySpatialQuery;

        HitCount = 0;
        _hitTargets = new HashSet<EnemyRuntime>();
        IsDead = false;
        
        AttackMovement.Initialize(this);
        
    }

    public void MarkDead()
    {
        IsDead = true;
    }

    public void SetTarget(EnemyRuntime target)
    {
        Target = target;
    }

    public bool TryHit(EnemyRuntime target)
    {
        if (target == null)
            return false;

        if (!_hitTargets.Add(target))
        {
            return false;
        }
        HitCount++;
        return true;
    }

    public bool HasHit(EnemyRuntime target)
    {
        return _hitTargets.Contains(target);
    }

    public void SetDirection(Vector2 direction)
    {
        Direction = direction.normalized;
    }
}