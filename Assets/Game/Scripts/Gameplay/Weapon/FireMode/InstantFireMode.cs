public class InstantFireMode : IFireMode
{
    public void Update(WeaponController controller, WeaponRuntime runtime, float deltaTime)
    {
        if (!controller.IsCooldownReady)
        {
            return;
        }

        controller.TryFireNow();
    }
}