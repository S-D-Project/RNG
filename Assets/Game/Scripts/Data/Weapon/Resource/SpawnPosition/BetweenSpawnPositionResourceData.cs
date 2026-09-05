using System;
using UnityEngine;

[Serializable]
public class BetweenSpawnPositionResourceData : SpawnPositionResourceData
{
    [Range(0f, 1f)]
    public float Ratio;

    public override ISpawnPosition Create()
    {
        return new BetweenSpawnPosition(Ratio, Offset);
    }
}