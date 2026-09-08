using System.Collections.Generic;
using UnityEngine;

public interface ITargeting
{
    bool RequiresTarget {get;}
    EnemyRuntime FindTarget(Vector2 origin, IEnemySpatialQuery spatialQuery);
}