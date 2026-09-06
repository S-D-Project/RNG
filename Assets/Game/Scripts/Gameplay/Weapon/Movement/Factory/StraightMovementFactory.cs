using UnityEngine;

public class StraightMovementFactory : IAttackMovementFactory
{
    public bool TryCreateMovement(Vector2 direction, out IAttackMovement movement)
    {
        movement = new StraightAttackMovement();

        return true;
    }
}