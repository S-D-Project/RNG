using System.Collections.Generic;
using UnityEngine;

public class NearestTargeting : ITargeting
{
    
    private readonly  List<EnemyRuntime> _candidates = new ();
    
    public bool RequiresTarget => true;
    
    public EnemyRuntime FindTarget(
        Vector2 origin,
        Vector2 forward,
        float searchRange,
        IEnemySpatialQuery spatialQuery)
    {
        spatialQuery.Query(origin, searchRange, _candidates);

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