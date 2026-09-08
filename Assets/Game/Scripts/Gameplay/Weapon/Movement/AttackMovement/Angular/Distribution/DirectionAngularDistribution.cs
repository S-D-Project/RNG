using UnityEngine;

public class DirectionAngularDistribution : IAngularDistribution
{
    public bool TryAcquire(Vector2 direction, out IAngularPhase phase)
    {
        float angle = Mathf.Atan2(
            direction.y, direction.x);

        phase = new LocalAngularPhase(angle);
        return true;
    }
}