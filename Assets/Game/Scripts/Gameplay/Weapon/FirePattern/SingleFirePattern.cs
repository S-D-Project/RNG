
using UnityEngine;

public class SingleFirePattern : IFirePattern
{
    public void Fire(WeaponController controller, WeaponRuntime runtime, EnemyRuntime target)
    {
        if (target == null)
        {
            return;
        }

        WeaponResourceData resource = runtime.BaseData.WeaponResourceData;

        if (resource.AttackPrefab == null)
        {
            Debug.LogError("AttackPrefab is null");
            return;
        }
        
        Vector2 origin = controller.transform.position;
        Vector2 targetPosition = target.transform.position;

        Vector2 direction = (targetPosition - origin).normalized;

        IMovement movement = resource.Movement.Create();

        controller.AttackRuntimeManager.Spawn(
            resource.AttackPrefab,
            origin,
            direction,
            runtime.CurrentSpeed,
            runtime.CurrentDamage,
            runtime.BaseData.HitRadius,
            runtime.BaseData.Lifetime,
            movement);
        
        Debug.Log("Fire");
    }
}