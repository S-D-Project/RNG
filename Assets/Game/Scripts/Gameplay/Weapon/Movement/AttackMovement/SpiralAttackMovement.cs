using UnityEngine;

public class SpiralAttackMovement : IAttackMovement
{
    private readonly float _radius;
    private readonly float _rotationSpeed;

    private Vector2 _initialDirection;
    private Vector2 _center;

    private float _angle;

    public SpiralAttackMovement(float radius, float rotationSpeed)
    {
        _radius = radius;
        _rotationSpeed = rotationSpeed;
    }
    
    public void Move(AttackRuntime attack, float deltaTime)
    {
        _center += _initialDirection * (attack.Speed * deltaTime);

        _angle += _rotationSpeed * Mathf.Deg2Rad * deltaTime;
        
        SetPosition(attack);
    }

    public void Initialize(AttackRuntime attack)
    {
        _initialDirection = attack.Direction.normalized;

        _center = attack.Transform.position;

        _angle = 0f;

        SetPosition(attack);
    }

    private void SetPosition(AttackRuntime attack)
    {
        Vector2 forward = _initialDirection;

        Vector2 right = new Vector2(forward.y, -forward.x);

        Vector2 offset = right * (Mathf.Cos(_angle) * _radius) + forward * (Mathf.Sin(_angle) * _radius);

        attack.Transform.position = _center + offset;
    }

    public void OnRelease(AttackRuntime attack)
    {
    }
}