
using UnityEngine;

public class PierceWeaponBehaviour : IWeaponBehaviour
{
    private readonly int _pierceCount;
    public PierceWeaponBehaviour(int count)
    {
        _pierceCount = count;
    }
    
    public void OnHit(AttackRuntime attack, EnemyRuntime target)
    {
        if (attack.HitCount > _pierceCount)
        {
            attack.MarkDead();
        }
    }
}