
using System.Collections.Generic;
using UnityEngine;

public class ForwardTargeting : ITargeting
{
    public bool RequiresTarget => true;

    public EnemyRuntime FindTarget(Vector2 origin, IEnemySpatialQuery spatialQuery)
    {
        throw new System.NotImplementedException();
    }
}