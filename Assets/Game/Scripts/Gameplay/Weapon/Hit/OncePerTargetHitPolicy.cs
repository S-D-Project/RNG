using System.Collections.Generic;

public class OncePerTargetHitPolicy : IHitPolicy
{
    private readonly HashSet<EnemyRuntime> _hitTargets = new();

    public void BeginFrame(float currentTime)
    {
    }

    public bool TryHit(EnemyRuntime target)
    {
        return _hitTargets.Add(target);
    }
}