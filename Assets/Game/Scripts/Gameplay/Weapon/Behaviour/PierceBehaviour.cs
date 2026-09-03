
using UnityEngine;

public class PierceBehaviour : IWeaponBehaviour
{
    private readonly int _pierceCount;
    public PierceBehaviour(int count)
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