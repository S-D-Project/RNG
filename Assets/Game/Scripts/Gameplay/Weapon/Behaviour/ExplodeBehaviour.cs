
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehaviour : IWeaponBehaviour
{
    private readonly float _explosionRadiusMultiplier;
    private readonly float _damageMultiplier;
    
    public ExplodeBehaviour(float explosionRadiusMultiplier, float damageMultiplier)
    {
        _explosionRadiusMultiplier = explosionRadiusMultiplier;
        _damageMultiplier = damageMultiplier;
    }
    
    public void OnHit(AttackRuntime attack, EnemyRuntime target)
    {
        float explosionRadius = attack.HitRadius * _explosionRadiusMultiplier;
        
        // TODO 적 서치 리팩토링 대상
        IReadOnlyList<EnemyRuntime> enemies = EnemyManager.Instance.EnemyList;

        Vector2 center = target.transform.position;
        
        float explosionDamage = attack.Damage * _damageMultiplier;

        foreach (EnemyRuntime enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            Vector2 enemyPosition = enemy.transform.position;

            float collisionRadius = explosionRadius + enemy.HitRaidus;

            float sqrDistance = (enemyPosition - center).sqrMagnitude;
            if (sqrDistance > collisionRadius * collisionRadius)
            {
                continue;
            }

            enemy.TakeDamage(explosionDamage);
        }
    }
}