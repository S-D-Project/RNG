
using System.Collections.Generic;
using UnityEngine;

public class HomingAttackMovement : IAttackMovement
{
    private readonly float _turnSpeed;
    private readonly float _searchInterval;
    private float _searchRange;

    private float _remainingSearchInterval;

    private readonly List<EnemyRuntime> _candidates = new();

    public HomingAttackMovement(float turnSpeed,float  searchInterval,float searchRange)
    {
        _turnSpeed = turnSpeed;
        _searchInterval = searchInterval;
        _remainingSearchInterval = 0f;
        _searchRange = searchRange;
    }
    
    public void Move(AttackRuntime attack, float deltaTime)
    {
        UpdateTarget(attack, deltaTime);

        if (attack.Target != null)
        {
            UpdateDirection(attack, attack.Target, deltaTime);
        }
        
        attack.Transform.position += (Vector3)attack.Direction * (attack.Speed * deltaTime);
    }

    public void Initialize(AttackRuntime attack)
    {
        
    }

    public void OnRelease(AttackRuntime attack)
    {
        
    }

    private void UpdateTarget(AttackRuntime attack, float deltaTime)
    {
        _remainingSearchInterval -= deltaTime;

        if (_remainingSearchInterval > 0f)
        {
            return;
        }

        _remainingSearchInterval = _searchInterval;
        attack.SetTarget(FindNearestTarget(attack));
    }

    private void UpdateDirection(AttackRuntime attack, EnemyRuntime target, float deltaTime)
    {
        Vector2 targetDirection = ((Vector2)target.transform.position - (Vector2)attack.Transform.position).normalized;

        float maxRadiansDelta = _turnSpeed * Mathf.Deg2Rad * deltaTime;

        Vector2 newDirection = Vector3.RotateTowards(attack.Direction, targetDirection, maxRadiansDelta, 0f);
        
        attack.SetDirection(newDirection);
    }

    private EnemyRuntime FindNearestTarget(AttackRuntime attack)
    {
        Vector2 position = attack.Transform.position;
        
        attack.EnemySpatialQuery.Query(position,_searchRange,_candidates);
        
        EnemyRuntime nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (EnemyRuntime enemy in _candidates)
        {
            Vector2 delta = (Vector2)enemy.transform.position - position;

            float sqrDistance = delta.sqrMagnitude;

            if (sqrDistance >= nearestSqrDistance)
            {
                continue;
            }

            nearest = enemy;
            nearestSqrDistance = sqrDistance;
        }

        return nearest;
    }
}