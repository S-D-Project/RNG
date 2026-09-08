using System.Collections;
using UnityEngine;

public class EnemyTestSpawner : MonoBehaviour
{
    [SerializeField]
    private EnemyRuntime _enemyPrefab;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private int _spawnCount = 500;

    [SerializeField]
    private int _spawnPerFrame = 20;

    [SerializeField]
    private float _minSpawnRadius = 10f;

    [SerializeField]
    private float _maxSpawnRadius = 30f;
    
    public IEnumerator Initialize(Transform target)
    {
        _target = target;
        int spawnedCount = 0;

        while (spawnedCount < _spawnCount)
        {
            int count =
                Mathf.Min(
                    _spawnPerFrame,
                    _spawnCount - spawnedCount);

            for (int i = 0; i < count; i++)
            {
                SpawnEnemy();
            }

            spawnedCount += count;

            yield return null;
        }
        
    }
    
    private void SpawnEnemy()
    {
        Vector2 position =
            GetRandomSpawnPosition();

        EnemyRuntime enemy =
            Instantiate(
                _enemyPrefab,
                position,
                Quaternion.identity);

        EnemyManager.Instance.Register(enemy);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        float minRadiusSqr =
            _minSpawnRadius * _minSpawnRadius;

        float maxRadiusSqr =
            _maxSpawnRadius * _maxSpawnRadius;

        float distance =
            Mathf.Sqrt(
                Random.Range(
                    minRadiusSqr,
                    maxRadiusSqr));

        return (Vector2)_target.position
               + direction * distance;
    }
}