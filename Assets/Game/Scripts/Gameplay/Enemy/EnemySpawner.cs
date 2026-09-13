using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Title("Spawn Settings")]
    [SerializeField]
    private float _minSpawnRadius = 10f;

    [SerializeField]
    private float _maxSpawnRadius = 30f;


    private Transform _target;
    private EnemyPool _enemyPool;


    public void Initialize(
        Transform target,
        EnemyPool enemyPool)
    {
        _target = target;
        _enemyPool = enemyPool;
    }


    public EnemyRuntime Spawn(EnemyData enemyData)
    {
        Vector2 position =
            GetRandomSpawnPosition();

        return Spawn(
            enemyData,
            position);
    }


    public EnemyRuntime Spawn(
        EnemyData enemyData,
        Vector2 position)
    {
        EnemyRuntime enemy =
            _enemyPool.Get(
                enemyData,
                position);

        EnemyManager.Instance.Register(enemy);

        return enemy;
    }


    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        float minRadiusSqr =
            _minSpawnRadius *
            _minSpawnRadius;

        float maxRadiusSqr =
            _maxSpawnRadius *
            _maxSpawnRadius;

        float distance =
            Mathf.Sqrt(
                Random.Range(
                    minRadiusSqr,
                    maxRadiusSqr));

        return (Vector2)_target.position
               + direction * distance;
    }
}