using System.Collections.Generic;
using UnityEngine;

public class NearestTargeting : ITargeting
{
    public EnemyRuntime FindTarget(
        Vector2 origin,
        IReadOnlyList<EnemyRuntime> enemies)
    {
        EnemyRuntime nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (EnemyRuntime enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            Vector2 enemyPosition = enemy.transform.position;

            float sqrDistance =
                (enemyPosition - origin).sqrMagnitude;

            if (sqrDistance >= nearestSqrDistance)
            {
                continue;
            }

            nearestSqrDistance = sqrDistance;
            nearest = enemy;
        }

        return nearest;
    }
}