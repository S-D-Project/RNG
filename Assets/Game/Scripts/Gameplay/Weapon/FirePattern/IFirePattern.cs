public interface IFirePattern
{
    void Fire(WeaponController controller,
        WeaponRuntime runtime,
        EnemyRuntime target);
    
}