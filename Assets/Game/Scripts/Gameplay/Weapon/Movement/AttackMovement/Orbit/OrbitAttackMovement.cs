using UnityEngine;

public class OrbitAttackMovement
    : IAttackMovement
{
    private readonly AngularCenterType _centerType;
    private readonly Vector2 _centerOffset;
    private readonly float _radius;
    private readonly IAngularPhase _phase;

    private Vector2 _spawnCenter;
    private Vector2 _initialDirection;

    private Vector2 _lastTargetPosition;

    public OrbitAttackMovement(
        AngularCenterType centerType,
        Vector2 centerOffset,
        float radius,
        IAngularPhase phase)
    {
        _centerType = centerType;
        _centerOffset = centerOffset;
        _radius = radius;
        _phase = phase;
    }

    public void Initialize(
        AttackRuntime attack)
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

        SetPosition(
            attack,
            _phase.CurrentAngle);
    }

    public void Move(
        AttackRuntime attack,
        float deltaTime)
    {
        if (_radius <= Mathf.Epsilon)
        {
            return;
        }

        float angularSpeed =
            attack.Speed / _radius;

        float angle =
            _phase.Advance(
                angularSpeed,
                deltaTime);

        SetPosition(
            attack,
            angle);
    }

    public void OnRelease(
        AttackRuntime attack)
    {
        _phase.Release();
    }

    private void SetPosition(
        AttackRuntime attack,
        float angle)
    {
        Vector2 radialDirection =
            new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle));

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