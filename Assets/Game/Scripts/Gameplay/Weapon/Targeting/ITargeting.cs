using System.Collections.Generic;
using UnityEngine;

public interface ITargeting
{
    EnemyRuntime FindTarget(Vector2 origin, IReadOnlyList<EnemyRuntime> enemies);
}