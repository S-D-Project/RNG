using System.Collections.Generic;
using UnityEngine;

public class RandomTargeting : ITargeting
{
    private readonly List<EnemyRuntime> _candidates = new();

    public bool RequiresTarget => true;

    public EnemyRuntime FindTarget(Vector2 origin,Vector2 forward, float searchRange, IEnemySpatialQuery spatialQuery)
    {
        spatialQuery.Query(origin, searchRange, _candidates);

        if (_candidates.Count == 0)
        {
            return null;
        }

        
        int randomIndex = Random.Range(0, _candidates.Count);

        return _candidates[randomIndex];
    }
}