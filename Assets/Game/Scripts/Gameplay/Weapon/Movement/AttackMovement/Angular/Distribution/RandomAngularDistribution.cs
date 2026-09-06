using UnityEngine;

public class RandomAngularDistribution : IAngularDistribution
{
    public bool TryAcquire(Vector2 direction, out IAngularPhase phase)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);

        phase = new LocalAngularPhase(angle);

        return true;
    }
}