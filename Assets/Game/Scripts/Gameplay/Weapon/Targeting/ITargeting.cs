using System.Collections.Generic;
using UnityEngine;

public interface ITargeting
{
    bool RequiresTarget {get;}
    EnemyRuntime FindTarget(Vector2 origin,Vector2 forward, float searchRange, IEnemySpatialQuery spatialQuery);
}