using System;

[Serializable]
public class OwnerSpawnPositionResourceData : SpawnPositionResourceData
{
    public override ISpawnPosition Create()
    {
        return new OwnerSpawnPosition(Offset);
    }
}