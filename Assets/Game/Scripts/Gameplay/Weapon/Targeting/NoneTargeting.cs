using UnityEngine;

public class NoneTargeting : ITargeting
{
    // 롤에 그 논타겟이 아니라 진짜 타겟이 없음
    public bool RequiresTarget => false;

    public EnemyRuntime FindTarget(Vector2 origin, IEnemySpatialQuery spatialQuery)
    {
        return null;
    }
}