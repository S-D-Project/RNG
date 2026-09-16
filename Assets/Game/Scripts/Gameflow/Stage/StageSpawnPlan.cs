using System.Collections.Generic;

public class StageSpawnPlan
{
    private readonly List<WaveSpawnPlan> _waveList;
    
    public IReadOnlyList<WaveSpawnPlan> WaveList => _waveList;

    public StageSpawnPlan(List<WaveSpawnPlan> waveList)
    {
        _waveList = waveList;
    }

    public Dictionary<string, EnemyPrewarmData>
        CalculatePrewarmData()
    {
        Dictionary<string, EnemyPrewarmData> result =
            new Dictionary<string, EnemyPrewarmData>();

        foreach (WaveSpawnPlan wave in _waveList)
        {
            foreach (WaveSpawnGroupPlan group
                     in wave.SpawnGroups)
            {
                foreach (EnemyData enemyData
                         in group.EnemyList)
                {
                    if (!result.TryGetValue(
                            enemyData.Id,
                            out EnemyPrewarmData prewarmData))
                    {
                        result.Add(
                            enemyData.Id,
                            new EnemyPrewarmData(
                                enemyData,
                                1));

                        continue;
                    }

                    prewarmData.Count++;
                }
            }
        }

        return result;
    }
    
    
}