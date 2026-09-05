using UnityEngine;

public class TargetSpawnPosition : SpawnPosition
{
    public TargetSpawnPosition(Vector2 offset) : base(offset)
    {
    }

    protected override Vector2 GetBasePosition(Vector2 ownerPosition, Vector2 targetPosition)
    {
        return targetPosition;
    }
}