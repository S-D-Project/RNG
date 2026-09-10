using System.Collections.Generic;
using UnityEngine;

public class ForwardTargeting : ITargeting
{
    private readonly float _searchAngle;
    private readonly float _minDot;

    private readonly List<EnemyRuntime> _candidates = new();

    public bool RequiresTarget => true;

    public ForwardTargeting(float searchAngle)
    {
        _searchAngle = searchAngle;

        float halfAngle =
            _searchAngle * 0.5f;

        _minDot =
            Mathf.Cos(halfAngle * Mathf.Deg2Rad);
    }

    public EnemyRuntime FindTarget(
        Vector2 origin,
        Vector2 forward,
        float searchRange,
        IEnemySpatialQuery spatialQuery)
    {
        spatialQuery.Query(
            origin,
            searchRange,
            _candidates);

        EnemyRuntime nearest = null;
        float nearestSqrDistance = float.MaxValue;

        Vector2 forwardDirection =
            forward.normalized;

        foreach (EnemyRuntime enemy in _candidates)
        {
            Vector2 delta =
                (Vector2)enemy.transform.position - origin;

            float sqrDistance =
                delta.sqrMagnitude;

            Vector2 direction =
                delta.normalized;

            float dot =
                Vector2.Dot(
                    forwardDirection,
                    direction);

            if (dot < _minDot)
            {
                continue;
            }

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