
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBehaviour : IWeaponBehaviour
{
    private readonly float _explosionRadiusMultiplier;
    private readonly float _damageMultiplier;

    private readonly List<EnemyRuntime> _candidates = new();
    
    public ExplodeBehaviour(float explosionRadiusMultiplier, float damageMultiplier)
    {
        _explosionRadiusMultiplier = explosionRadiusMultiplier;
        _damageMultiplier = damageMultiplier;
    }
    
    public void OnHit(AttackRuntime attack, EnemyRuntime target)
    {
        float explosionRadius = attack.HitRadius * _explosionRadiusMultiplier;

        Vector2 center = target.transform.position;

        attack.EnemySpatialQuery.QueryOverlapCircle(center, explosionRadius, _candidates);
        
        
        float explosionDamage = attack.Damage * _damageMultiplier;

        foreach (EnemyRuntime enemy in _candidates)
        {
            enemy.TakeDamage(explosionDamage);
        }
        
    }
}