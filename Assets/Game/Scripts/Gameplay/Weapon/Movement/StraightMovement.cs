
using UnityEngine;

public class StraightMovement : IMovement
{
    public void Move(AttackRuntime attack, float deltaTime)
    {
        Vector2 moveAmount = attack.Direction * (attack.Speed * deltaTime);
        
        attack.Transform.position += (Vector3)moveAmount;
    }
}