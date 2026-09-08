using UnityEngine;

public class FollowMovementFactory : IAttackMovementFactory
{
    private readonly FollowTargetType _targetType;
    private readonly FollowMode _followMode;
    private readonly float _followSpeed;
    private readonly Vector2 _offset;
    public FollowMovementFactory(FollowTargetType targetType, FollowMode followMode, float followSpeed,Vector2 offset)
    {
        _targetType = targetType;
        _followMode = followMode;
        _followSpeed = followSpeed;
        _offset = offset;
    }
    
    public bool TryCreateMovement(Vector2 direction, out IAttackMovement movement)
    {
        movement = new FollowAttackMovement(_targetType, _followMode, _followSpeed,_offset);
        return true;
    }
}