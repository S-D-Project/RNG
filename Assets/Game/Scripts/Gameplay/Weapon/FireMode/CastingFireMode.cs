public class CastingFireMode : IFireMode
{
    private float _castingTime = 0f;
    public void Update(WeaponController controller, WeaponRuntime runtime, float deltaTime)
    {
        if (controller.IsOwnerMoving || !controller.HasTarget() || !controller.IsCooldownReady)
        {
            ResetCasting();
            return;
        }
        
        _castingTime += deltaTime;
        if (_castingTime < runtime.CurrentCastTime)
        {
            return;
        }

        controller.TryFireNow();
        ResetCasting();
        

    }

    private void ResetCasting()
    {
        _castingTime = 0f;
    }
}