using UnityEngine;

public class ExpandingOrbitMovementFactory : IAttackMovementFactory
{
    private readonly AngularCenterType _centerType;
    private readonly Vector2 _centerOffset;
    private readonly float _initialRadius;
    private readonly float _radialSpeed;

    public ExpandingOrbitMovementFactory(AngularCenterType centerType, Vector2 centerOffset, float initialRadius,
        float radialSpeed)
    {
        _centerType = centerType;
        _centerOffset = centerOffset;
        _initialRadius = initialRadius;
        _radialSpeed = radialSpeed;
    }

    public bool TryCreateMovement(Vector2 direction, out IAttackMovement movement)
    {
        movement = new ExpandingOrbitAttackMovement(
            _centerType,
            _centerOffset,
            _initialRadius,
            _radialSpeed);

        return true;
    }
}