using System.Collections;
using UnityEngine;

public class EnemyTestSpawner : MonoBehaviour
{
    [SerializeField]
    private int _spawnPerFrame = 20;

    public IEnumerator Spawn(
        EnemySpawner spawner,
        EnemyData enemyData,
        int spawnCount)
    {
        int spawnedCount = 0;

        while (spawnedCount < spawnCount)
        {
            int count =
                Mathf.Min(
                    _spawnPerFrame,
                    spawnCount - spawnedCount);

            for (int i = 0; i < count; i++)
            {
                spawner.Spawn(enemyData);
            }

            spawnedCount += count;

            yield return null;
        }
    }
}