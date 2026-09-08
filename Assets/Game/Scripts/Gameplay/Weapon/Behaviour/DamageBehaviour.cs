public class DamageBehaviour : IWeaponBehaviour
{
    public void OnHit(AttackRuntime attack, EnemyRuntime target)
    {
        target.TakeDamage(attack.Damage);
    }
}