using System.Collections.Generic;
using UnityEngine;

public class AttackRuntime
{
    private readonly HashSet<EnemyRuntime> _hitTargets;
    
    public GameObject Prefab { get; }
    public GameObject Instance { get; }

    public Transform Transform => Instance.transform;

    public Vector2 Direction { get; }
    public float Speed { get; }

    public float Damage { get; }
    public float HitRadius { get; }

    public float RemainingLifetime { get; set; }
    
    public int HitCount { get; private set; }

    public IMovement Movement { get; }
    public IReadOnlyList<IWeaponBehaviour> Behaviours { get; }

    public bool IsDead { get; private set; }

    public AttackRuntime(
        GameObject prefab,
        GameObject instance,
        Vector2 direction,
        float speed,
        float damage,
        float hitRadius,
        float lifetime,
        IMovement movement,
        IReadOnlyList<IWeaponBehaviour> behaviours)
    {
        Prefab = prefab;
        Instance = instance;

        Direction = direction.normalized;
        Speed = speed;

        Damage = damage;
        HitRadius = hitRadius;

        RemainingLifetime = lifetime;
        Movement = movement;
        Behaviours = behaviours;

        HitCount = 0;
        _hitTargets = new HashSet<EnemyRuntime>();
        IsDead = false;
    }

    public void MarkDead()
    {
        IsDead = true;
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
}