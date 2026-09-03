
using System.Collections.Generic;
using UnityEngine;

public class HomingMovement : IMovement
{
    private readonly float _turnSpeed;
    private readonly float _searchInterval;

    private float _remainingSearchInterval;
    private EnemyRuntime _currentTarget;

    public HomingMovement(float turnSpeed,float  searchInterval)
    {
        _turnSpeed = turnSpeed;
        _searchInterval = searchInterval;
        _remainingSearchInterval = 0f;
    }
    
    public void Move(AttackRuntime attack, float deltaTime)
    {
        UpdateTarget(attack, deltaTime);

        if (_currentTarget != null)
        {
            UpdateDirection(attack, _currentTarget, deltaTime);
        }
        
        attack.Transform.position += (Vector3)attack.Direction * (attack.Speed * deltaTime);
    }

    private void UpdateTarget(AttackRuntime attack, float deltaTime)
    {
        _remainingSearchInterval -= deltaTime;

        if (_remainingSearchInterval > 0f)
        {
            return;
        }

        _remainingSearchInterval = _searchInterval;
        _currentTarget = FindNearestTarget(attack);
        
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
        IReadOnlyList<EnemyRuntime> enemies = EnemyManager.Instance.EnemyList;

        EnemyRuntime nearest = null;
        float nearestSqrDistance = float.MaxValue;

        Vector2 position = attack.Transform.position;

        foreach (EnemyRuntime enemy in enemies)
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