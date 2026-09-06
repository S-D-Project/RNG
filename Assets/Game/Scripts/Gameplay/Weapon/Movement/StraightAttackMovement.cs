
using UnityEngine;

public class StraightAttackMovement : IAttackMovement
{
    public void Move(AttackRuntime attack, float deltaTime)
    {
        Vector2 moveAmount = attack.Direction * (attack.Speed * deltaTime);
        
        attack.Transform.position += (Vector3)moveAmount;
    }

    public void Initialize(AttackRuntime attack)
    {
        
    }
}