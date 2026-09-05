using UnityEngine;

public class OwnerSpawnPosition : SpawnPosition
{
    public OwnerSpawnPosition(Vector2 offset) : base(offset)
    {
    }

    protected override Vector2 GetBasePosition(Vector2 ownerPosition, Vector2 targetPosition)
    {
        return ownerPosition;
    }
}