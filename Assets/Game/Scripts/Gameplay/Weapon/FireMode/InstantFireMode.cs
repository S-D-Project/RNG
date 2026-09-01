public class InstantFireMode : IFireMode
{
    public void Fire(WeaponController controller, WeaponRuntime runtime, EnemyRuntime target)
    {
        controller.Fire(target);
    }
}