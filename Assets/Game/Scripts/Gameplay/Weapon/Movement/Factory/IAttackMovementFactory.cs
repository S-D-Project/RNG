using UnityEngine;
public interface IAttackMovementFactory
{
    bool TryCreateMovement(Vector2 direction, out IAttackMovement movement);
}