
using System.Collections.Generic;
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
        List<IWeaponBehaviour> behaviours = new List<IWeaponBehaviour>();

        foreach (BehaviourResourceData behaviourResourceData in resource.Behaviours)
        {
            behaviours.Add(behaviourResourceData.Create());
        }

        controller.AttackRuntimeManager.Spawn(
            resource.AttackPrefab,
            origin,
            direction,
            runtime.CurrentSpeed,
            runtime.CurrentDamage,
            runtime.BaseData.HitRadius,
            runtime.BaseData.Lifetime,
            movement,
            behaviours);
        
        Debug.Log("Fire");
    }
}