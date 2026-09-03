
using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    [SerializeField]
    private string _enemyName;

    [SerializeField]
    private float _hitRaiuds = 0.5f;

    public string EnemyName => _enemyName;
    public float HitRaidus => _hitRaiuds;
}