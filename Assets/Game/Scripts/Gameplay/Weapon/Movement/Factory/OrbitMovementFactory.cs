using UnityEngine;

public class OrbitMovementFactory
    : IAttackMovementFactory
{
    private readonly OrbitCenterType _centerType;
    private readonly Vector2 _centerOffset;
    private readonly float _radius;

    private readonly IAngularDistribution _distribution;

    public OrbitMovementFactory(
        OrbitCenterType centerType,
        Vector2 centerOffset,
        float radius,
        IAngularDistribution distribution)
    {
        _centerType = centerType;
        _centerOffset = centerOffset;
        _radius = radius;
        _distribution = distribution;
    }

    public bool TryCreateMovement(
        Vector2 direction,
        out IAttackMovement movement)
    {
        if (!_distribution.TryAcquire(
                direction,
                out IAngularPhase phase))
        {
            movement = null;

            return false;
        }

        movement =
            new OrbitAttackMovement(
                _centerType,
                _centerOffset,
                _radius,
                phase);

        return true;
    }
}