using UnityEngine;

public class OrbitMovement : IMovement
{
    private readonly OrbitCenterType _centerType;
    private readonly Vector2 _centerOffset;
    private readonly float _radius;

    private Vector2 _spawnCenter;
    private Vector2 _initialDirection;
    
    private float _angle;

    public OrbitMovement(OrbitCenterType orbitCenterType, Vector2 offset, float radius)
    {
        _centerType = orbitCenterType;
        _centerOffset = offset;
        _radius = radius;
    }

    public void Move(AttackRuntime attack, float deltaTime)
    {
        if (_radius <= Mathf.Epsilon)
        {
            return;
        }

        float angularSpeed = attack.Speed / _radius;

        _angle += angularSpeed * deltaTime;

        Vector2 radialDirection = new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle));

        Vector2 center = GetCenter(attack);
        attack.Transform.position = center + radialDirection * _radius;
    }

    public void Initialize(AttackRuntime attack)
    {
        _initialDirection = attack.Direction.normalized;
        _spawnCenter = attack.Transform.position;
        _angle = Mathf.Atan2(_initialDirection.y,_initialDirection.x);

        Vector2 center = GetCenter(attack);

        Vector2 radialDirection = new Vector2(Mathf.Cos(_angle),Mathf.Sin(_angle));
        
        attack.Transform.position = center + radialDirection * _radius;
    }

    private Vector2 GetCenter(AttackRuntime attack)
    {
        Vector2 baseCenter = _centerType switch
        {
            OrbitCenterType.SpawnPosition => _spawnCenter,
            OrbitCenterType.Owner => attack.Owner.position,
            OrbitCenterType.Target => attack.Target != null ? attack.Target.transform.position : _spawnCenter,
            _ => _spawnCenter
        };

        return baseCenter + GetCenterOffset();
    }

    private Vector2 GetCenterOffset()
    {
        Vector2 forward = _initialDirection;

        Vector2 right = new Vector2(forward.y, -forward.x);
        
        return right * _centerOffset.x + forward *_centerOffset.y;
    }
}