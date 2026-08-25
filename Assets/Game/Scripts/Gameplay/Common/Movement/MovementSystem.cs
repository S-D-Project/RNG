using Sirenix.OdinInspector;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    private Vector2 _moveDirection;
    
    public float MoveSpeed { get; private set; }


    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 movement = new Vector3(_moveDirection.x, _moveDirection.y, 0f);

        transform.position += movement * (MoveSpeed * Time.fixedDeltaTime);
    }

    public void SetMoveDirection(Vector2 moveDirection)
    {
        _moveDirection = moveDirection;
    }

    public void SetMoveSpeed(float moveSpeed)
    {
        MoveSpeed = moveSpeed;
    }
}