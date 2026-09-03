public class DestroyOnHitWeaponBehaviour : IWeaponBehaviour
{
    public void OnHit(AttackRuntime attack, EnemyRuntime target)
    {
        attack.MarkDead();
    }
}