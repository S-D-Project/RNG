using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AttackRuntimeManager : MonoBehaviour
{
    private readonly List<AttackRuntime> _attacks = new();

    private readonly Dictionary<GameObject, ObjectPool<GameObject>>
        _pools = new();

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        UpdateAttacks(deltaTime);
        CleanupAttacks();
    }

    public AttackRuntime Spawn(
        GameObject prefab,
        Vector2 position,
        Vector2 direction,
        float speed,
        float damage,
        float hitRadius,
        float lifetime,
        IMovement movement,
        IReadOnlyList<IWeaponBehaviour> behaviours)
    {
        if (prefab == null)
        {
            Debug.LogError("Attack prefab is null.");
            return null;
        }

        ObjectPool<GameObject> pool = GetOrCreatePool(prefab);

        GameObject instance = pool.Get();

        instance.transform.position = position;
        instance.transform.rotation = Quaternion.identity;

        AttackRuntime attack = new AttackRuntime(
            prefab,
            instance,
            direction,
            speed,
            damage,
            hitRadius,
            lifetime,
            movement,
            behaviours);

        _attacks.Add(attack);

        return attack;
    }

    private void UpdateAttacks(float deltaTime)
    {
        foreach (AttackRuntime attack in _attacks)
        {
            if (attack.IsDead)
            {
                continue;
            }

            UpdateMovement(attack, deltaTime);
            UpdateLifetime(attack, deltaTime);

            if (attack.IsDead)
            {
                continue;
            }
            
            UpdateCollision(attack);
        }
    }

    private void UpdateMovement(
        AttackRuntime attack,
        float deltaTime)
    {
        attack.Movement.Move(
            attack,
            deltaTime);
    }

    private void UpdateLifetime(
        AttackRuntime attack,
        float deltaTime)
    {
        attack.RemainingLifetime -= deltaTime;

        if (attack.RemainingLifetime > 0f)
        {
            return;
        }

        attack.MarkDead();
    }

    private void CleanupAttacks()
    {
        for (int i = _attacks.Count - 1; i >= 0; i--)
        {
            AttackRuntime attack = _attacks[i];

            if (!attack.IsDead)
            {
                continue;
            }

            Release(attack);

            _attacks.RemoveAt(i);
        }
    }

    private void UpdateCollision(AttackRuntime attack)
    {
        IReadOnlyList<EnemyRuntime> enemies = EnemyManager.Instance.EnemyList;

        Vector2 attackPosition = attack.Transform.position;

        foreach (EnemyRuntime enemy in enemies)
        {
            if(enemy == null)
            {
                continue;
            }

            if (attack.HasHit(enemy))
            {
                continue;
            }
            Vector2 enemyPosition = enemy.transform.position;

            float collisionRadius = attack.HitRadius + enemy.HitRaidus;

            float sqrDistance = (enemyPosition - attackPosition).sqrMagnitude;

            if (sqrDistance > collisionRadius * collisionRadius)
            { 
                continue;
            }

            if (!attack.TryHit(enemy))
            {
                continue;
            }
            
            HandleHit(attack, enemy);

            if (attack.IsDead)
            {
                break;
            }
        }
    }

    private void HandleHit(AttackRuntime attack, EnemyRuntime enemy)
    {
        foreach (IWeaponBehaviour behaviour in attack.Behaviours)
        {
            behaviour.OnHit(attack,enemy);
        }
    }

    private void Release(AttackRuntime attack)
    {
        if (!_pools.TryGetValue(
                attack.Prefab,
                out ObjectPool<GameObject> pool))
        {
            Debug.LogError(
                $"Attack pool was not found : {attack.Prefab.name}");

            Destroy(attack.Instance);
            return;
        }

        pool.Release(attack.Instance);
    }

    private ObjectPool<GameObject> GetOrCreatePool(
        GameObject prefab)
    {
        if (_pools.TryGetValue(
                prefab,
                out ObjectPool<GameObject> pool))
        {
            return pool;
        }

        pool = CreatePool(prefab);

        _pools.Add(prefab, pool);

        return pool;
    }

    private ObjectPool<GameObject> CreatePool(
        GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject instance =
                    Instantiate(prefab);

                instance.SetActive(false);

                return instance;
            },

            actionOnGet: instance =>
            {
                instance.SetActive(true);
            },

            actionOnRelease: instance =>
            {
                instance.SetActive(false);
            },

            actionOnDestroy: instance =>
            {
                Destroy(instance);
            },

            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 1000);
    }
}