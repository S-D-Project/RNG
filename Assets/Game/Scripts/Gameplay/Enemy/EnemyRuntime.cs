
using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    [SerializeField]
    private string _enemyName;

    [SerializeField]
    private float _hitRaiuds = 0.5f;

    [SerializeField]
    private float _moveSpeed;

    public string EnemyName => _enemyName;
    public float HitRadius => _hitRaiuds;
    public float MoveSpeed => _moveSpeed;

    public void TakeDamage(float damage)
    {
        Debug.Log($"{_enemyName} TakeDamage : {damage}");
    }
}