using UnityEngine;

public abstract class SpawnPosition : ISpawnPosition
{
    // 월드 좌표 
    private readonly Vector2 _offset;

    protected SpawnPosition(Vector2 offset)
    {
        _offset = offset;
    }

    public Vector2 GetPosition(Vector2 ownerPosition, Vector2 targetPosition, Vector2 direction)
    {
        Vector2 basePosition = GetBasePosition(ownerPosition, targetPosition);

        Vector2 rotatedOffset = GetRotatedOffset(direction);

        return basePosition + rotatedOffset;
    }

    protected abstract Vector2 GetBasePosition(Vector2 ownerPosition, Vector2 targetPosition);

    private Vector2 GetRotatedOffset(Vector2 direction)
    {
        Vector2 forward = direction.normalized;
        Vector2 right = new Vector2(forward.y, -forward.x);

        return right * _offset.x + forward * _offset.y;
    }
}