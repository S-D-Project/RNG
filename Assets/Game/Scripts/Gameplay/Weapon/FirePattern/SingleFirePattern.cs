
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

        Vector2 origin = controller.transform.position;
        Vector2 targetPosition = target.transform.position;
        Vector2 direction = (targetPosition - origin).normalized;

        controller.FireAttack(direction);
    }
}