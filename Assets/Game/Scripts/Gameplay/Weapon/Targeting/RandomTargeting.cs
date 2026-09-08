using System;
using UnityEngine;

public class RandomTargeting : ITargeting
{
    public bool RequiresTarget => true;

    public EnemyRuntime FindTarget(Vector2 origin, IEnemySpatialQuery spatialQuery)
    {
        throw new NotImplementedException();
    }
}