using UnityEngine;

public class AttackRuntime
{
    public GameObject Prefab { get; }
    public GameObject Instance { get; }

    public Transform Transform => Instance.transform;

    public Vector2 Direction { get; }
    public float Speed { get; }

    public float Damage { get; }
    public float HitRadius { get; }

    public float RemainingLifetime { get; set; }

    public IMovement Movement { get; }

    public bool IsDead { get; private set; }

    public AttackRuntime(
        GameObject prefab,
        GameObject instance,
        Vector2 direction,
        float speed,
        float damage,
        float hitRadius,
        float lifetime,
        IMovement movement)
    {
        Prefab = prefab;
        Instance = instance;

        Direction = direction.normalized;
        Speed = speed;

        Damage = damage;
        HitRadius = hitRadius;

        RemainingLifetime = lifetime;
        Movement = movement;

        IsDead = false;
    }

    public void MarkDead()
    {
        IsDead = true;
    }
}