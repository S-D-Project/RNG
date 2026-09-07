using System.Collections.Generic;
using UnityEngine;

public class NearestTargeting : ITargeting
{

    private readonly  float _searchRange;
    private readonly  List<EnemyRuntime> _candidates = new ();

    public NearestTargeting(float searchRange)
    {
        _searchRange = searchRange;
    }
    
    public EnemyRuntime FindTarget(
        Vector2 origin,
        IEnemySpatialQuery spatialQuery)
    {
        spatialQuery.Query(origin, _searchRange, _candidates);

        EnemyRuntime nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (EnemyRuntime enemy in _candidates)
        {
            Vector2 enemyPosition = enemy.transform.position;

            float sqrDistacne = (enemyPosition - origin).sqrMagnitude;

            if (sqrDistacne >= nearestSqrDistance)
            {
                continue;
            }

            nearestSqrDistance = sqrDistacne;

            nearest = enemy;
        }

        return nearest;
    }
}