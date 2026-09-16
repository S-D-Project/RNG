using System;
using System.Collections.Generic;

[Serializable]
public class WaveData
{
    public int WaveNumber;
    public float MaxDuration;
    public int KillThreshold;
    public List<WaveSpawnData> SpawnList;
}