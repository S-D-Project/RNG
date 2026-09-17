using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    
    public float ContactDamage { get; private set; }
    
    public float CurrentHp { get; private set; }
    public bool IsDead { get; private set; }


    public float HitRadius { get; private set; }
    public float MoveSpeed { get; private set; }
    
    public void Initialize(EnemyData data)
    {
        Data = data;
        MoveSpeed = data.MoveSpeed;
        CurrentHp = data.MaxHp;
        ContactDamage = data.ContactDamage;
        HitRadius =  data.HitRadius;
        IsDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
        {
            return;
        }

        CurrentHp -= damage;

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        CurrentHp = 0f;
        IsDead = true;
    }
}