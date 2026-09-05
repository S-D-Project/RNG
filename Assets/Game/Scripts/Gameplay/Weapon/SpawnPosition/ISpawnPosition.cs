using UnityEngine;

public interface ISpawnPosition
{
    Vector2 GetPosition(Vector2 ownerPosition,Vector2 targetPosition,Vector2 direction);
}