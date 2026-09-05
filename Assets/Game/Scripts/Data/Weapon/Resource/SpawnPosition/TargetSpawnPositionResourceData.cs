using System;

[Serializable]
public class TargetSpawnPositionResourceData : SpawnPositionResourceData
{
    public override ISpawnPosition Create()
    {
        return new TargetSpawnPosition(Offset);
    }
}