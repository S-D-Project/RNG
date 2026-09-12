using UnityEngine;

public class FollowAttackMovement : IAttackMovement
{
    private readonly FollowTargetType _targetType;
    private readonly FollowMode _followMode;
    private readonly float _followSpeed;
    private readonly Vector2 _offset;

    private Vector2 _lastTargetPosition;

    public FollowAttackMovement(
        FollowTargetType targetType,
        FollowMode followMode,
        float followSpeed,
        Vector2 offset)
    {
        _targetType = targetType;
        _followMode = followMode;
        _followSpeed = followSpeed;
        _offset = offset;
    }

    public void Initialize(AttackRuntime attack)
    {
        _lastTargetPosition = GetTargetPosition(attack);
    }

    public void Move(
        AttackRuntime attack,
        float deltaTime)
    {
        Vector2 targetPosition =
            GetTargetPosition(attack);

        _lastTargetPosition =
            targetPosition;

        if (_followMode == FollowMode.Snap)
        {
            attack.Transform.position =
                targetPosition;

            return;
        }

        Vector2 currentPosition =
            attack.Transform.position;

        float t =
            1f - Mathf.Exp(
                -_followSpeed * deltaTime);

        attack.Transform.position =
            Vector2.Lerp(
                currentPosition,
                targetPosition,
                t);
    }

    public void OnRelease(AttackRuntime attack)
    {
    }

    private Vector2 GetTargetPosition(
        AttackRuntime attack)
    {
        switch (_targetType)
        {
            case FollowTargetType.Owner:
                return (Vector2)attack.Owner.position + _offset;

            case FollowTargetType.Target:
                if (attack.Target != null)
                {
                    return (Vector2)attack.Target.transform.position + _offset;
                }

                return _lastTargetPosition;

            default:
                return _lastTargetPosition;
        }
    }
}