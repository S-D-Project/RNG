using Sirenix.OdinInspector;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    private Vector2 _moveDirection;
    
    public float MoveSpeed { get; private set; }
    
    public void Move(float deltaTime)
    {
        Vector3 movement = new Vector3(_moveDirection.x, _moveDirection.y, 0f);

        transform.position += movement * (MoveSpeed *deltaTime);
    }

    public void SetMoveDirection(Vector2 moveDirection)
    {
        _moveDirection = Vector2.ClampMagnitude(moveDirection, 1f);
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        MoveSpeed = moveSpeed;
    }
}