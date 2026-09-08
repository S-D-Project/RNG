using System;
using UnityEngine;

[Serializable]
public abstract class SpawnPositionResourceData
{
    public Vector2 Offset;

    public abstract ISpawnPosition Create();
}