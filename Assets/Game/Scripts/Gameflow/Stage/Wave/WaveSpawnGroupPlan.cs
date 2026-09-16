using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnGroupPlan
{
    private readonly List<EnemyData> _enemyList;
    public IReadOnlyList<EnemyData> EnemyList => _enemyList;

    public float SpawnDuration { get; }
    public SpawnTimingType SpawnTiming { get; }

    public WaveSpawnGroupPlan(List<EnemyData> enemyList, float spawnDuration, SpawnTimingType spawnTiming)
    {
        _enemyList = enemyList;
        SpawnDuration = spawnDuration;
        SpawnTiming = spawnTiming;
    }

    private float EvaluateSpawnProgress(SpawnTimingType timing, float normalizedTime)
    {
        normalizedTime = Mathf.Clamp01(normalizedTime);

        switch (timing)
        {
            case SpawnTimingType.Constant:
                return normalizedTime;
            
            case SpawnTimingType.Accelerating:
                return normalizedTime * normalizedTime;
            
            case SpawnTimingType.Decelerating:
                float inverse = 1 - normalizedTime;
                return 1f - inverse * inverse;
            
            default:
                return normalizedTime;
        }
    }
}