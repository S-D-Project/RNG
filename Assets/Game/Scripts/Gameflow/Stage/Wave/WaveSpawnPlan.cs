using System.Collections.Generic;

public class WaveSpawnPlan
{

    public int WaveNumber { get; }
    public int KillThreshold { get; }
    public float MaxDuration { get; }
    
    private readonly List<WaveSpawnGroupPlan> _spawnGroups;
    public IReadOnlyList<WaveSpawnGroupPlan> SpawnGroups => _spawnGroups;

    public WaveSpawnPlan(int waveNumber,int killThreshold,float maxDuration,
        List<WaveSpawnGroupPlan> spawnGroups)
    {
        WaveNumber = waveNumber;
        KillThreshold = killThreshold;
        MaxDuration = maxDuration;
        _spawnGroups = spawnGroups;
    }
}