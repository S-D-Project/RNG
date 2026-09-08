public class CastingFireMode : IFireMode
{
    private float _castingTime = 0f;
    public void Update(WeaponController controller, WeaponRuntime runtime, float deltaTime)
    {
        if (controller.IsOwnerMoving  || !controller.IsCooldownReady)
        {
            ResetCasting();
            return;
        }

        if (!controller.TryFindTarget(out EnemyRuntime target))
        {
            ResetCasting();
            return;
        }
        
        _castingTime += deltaTime;
        if (_castingTime < runtime.CurrentCastTime)
        {
            return;
        }

        controller.TryFireNow(target);
        ResetCasting();
    }

    private void ResetCasting()
    {
        _castingTime = 0f;
    }
}