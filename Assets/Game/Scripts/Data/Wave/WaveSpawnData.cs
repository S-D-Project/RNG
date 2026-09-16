using System;

[Serializable]
public class WaveSpawnData
{
    public EnemySpawnType SpawnType;
    public string EnemyId;
    public EnemyGrade Grade;
    public int Count;
    public float SpawnDuration;
    public SpawnTimingType SpawnTiming;
}

public enum EnemySpawnType
{
    Fixed,
    RandomGrade
}