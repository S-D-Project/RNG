public class DestroyBehaviour : IWeaponBehaviour
{
    public void OnHit(AttackRuntime attack, EnemyRuntime target)
    {
        attack.MarkDead();
    }
}