using System.Collections.Generic;
using UnityEngine;

public interface IEnemySpatialQuery
{
    void Query(Vector2 center, float radius, List<EnemyRuntime> results);

    void QueryOverlapCircle(Vector2 center, float radius, List<EnemyRuntime> results);
    
}