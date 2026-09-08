public interface IHitPolicy
{
    void BeginFrame(float currentTime);
    bool TryHit(EnemyRuntime target);
}
