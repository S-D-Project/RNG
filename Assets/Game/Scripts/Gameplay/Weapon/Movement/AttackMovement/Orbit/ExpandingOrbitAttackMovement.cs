using UnityEngine;

public class ExpandingOrbitAttackMovement : IAttackMovement
{
    private readonly AngularCenterType _centerType;
    private readonly Vector2 _centerOffset;
    private readonly float _radialSpeed;

    private Vector2 _spawnCenter;
    private Vector2 _initialDirection;
    private Vector2 _lastTargetPosition;

    private float _radius;
    private float _angle;

    public ExpandingOrbitAttackMovement(
        AngularCenterType centerType,
        Vector2 centerOffset,
        float initialRadius,
        float radialSpeed)
    {
        _centerType = centerType;
        _centerOffset = centerOffset;

        _radius = initialRadius;
        _radialSpeed = radialSpeed;
    }

    public void Initialize(AttackRuntime attack)
    {
        _initialDirection =
            attack.Direction.normalized;

        _spawnCenter =
            attack.Transform.position;

        if (_centerType == AngularCenterType.Target)
        {
            _lastTargetPosition =
                attack.Target.transform.position;
        }

        _angle = Mathf.Atan2(
            _initialDirection.y,
            _initialDirection.x);

        SetPosition(attack);
    }

    public void Move(
        AttackRuntime attack,
        float deltaTime)
    {
        _radius +=
            _radialSpeed * deltaTime;

        if (_radius <= Mathf.Epsilon)
        {
            attack.MarkDead();
            return;
        }

        float angularSpeed =
            attack.Speed / _radius;

        _angle +=
            angularSpeed * deltaTime;

        SetPosition(attack);
    }

    public void OnRelease(AttackRuntime attack)
    {
    }

    private void SetPosition(
        AttackRuntime attack)
    {
        Vector2 radialDirection =
            new Vector2(
                Mathf.Cos(_angle),
                Mathf.Sin(_angle));

        Vector2 center =
            GetCenter(attack);

        attack.Transform.position =
            center
            + radialDirection * _radius;
    }

    private Vector2 GetCenter(
        AttackRuntime attack)
    {
        Vector2 baseCenter =
            _centerType switch
            {
                AngularCenterType.SpawnPosition =>
                    _spawnCenter,

                AngularCenterType.Owner =>
                    attack.Owner.position,

                AngularCenterType.Target =>
                    GetTargetCenter(attack),

                _ => _spawnCenter
            };

        return baseCenter
               + GetCenterOffset();
    }

    private Vector2 GetTargetCenter(
        AttackRuntime attack)
    {
        if (attack.Target != null)
        {
            _lastTargetPosition =
                attack.Target.transform.position;
        }

        return _lastTargetPosition;
    }

    private Vector2 GetCenterOffset()
    {
        Vector2 forward =
            _initialDirection;

        Vector2 right =
            new Vector2(
                forward.y,
                -forward.x);

        return right * _centerOffset.x
               + forward * _centerOffset.y;
    }
}