using UnityEngine;

public class WaveAttackMovement : IAttackMovement
{
    private readonly float _amplitude;
    private readonly float _frequency;

    private Vector2 _initialDirection;
    private Vector2 _right;
    private Vector2 _center;

    private float _phase;

    public WaveAttackMovement(float amplitude, float frequency)
    {
        _amplitude = amplitude;
        _frequency = frequency;
    }
    
    public void Move(AttackRuntime attack, float deltaTime)
    {
        _center += _initialDirection * (attack.Speed * deltaTime);

        _phase += Mathf.PI * 2f * _frequency * deltaTime;

        Vector2 waveOffset = _right * (Mathf.Sin(_phase) * _amplitude);

        attack.Transform.position = _center + waveOffset;
    }

    public void Initialize(AttackRuntime attack)
    {
        _initialDirection = attack.Direction.normalized;

        _right = new Vector2(_initialDirection.y, -_initialDirection.x);

        _center = attack.Transform.position;

        _phase = 0f;
    }

    public void OnRelease(AttackRuntime attack)
    {
    }
}