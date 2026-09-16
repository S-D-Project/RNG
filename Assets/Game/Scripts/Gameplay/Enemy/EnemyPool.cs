using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [Title("Pool Settings")]
    [SerializeField]
    private int _defaultCapacity = 20;

    [SerializeField]
    private int _maxSize = 500;


    private readonly Dictionary<string, ObjectPool<EnemyRuntime>> _pools = new();
    
    public EnemyRuntime Get(
        EnemyData enemyData,
        Vector2 position)
    {
        ObjectPool<EnemyRuntime> pool =
            GetOrCreatePool(enemyData);

        EnemyRuntime enemy =
            pool.Get();

        enemy.transform.SetPositionAndRotation(
            position,
            Quaternion.identity);

        enemy.Initialize(enemyData);

        enemy.gameObject.SetActive(true);

        return enemy;
    }


    public void Release(EnemyRuntime enemy)
    {
        if (enemy == null || enemy.Data == null)
        {
            return;
        }

        string enemyId = enemy.Data.Id;

        if (!_pools.TryGetValue(
                enemyId,
                out ObjectPool<EnemyRuntime> pool))
        {
            Debug.LogError(
                $"Enemy Pool not found. Id: {enemyId}");

            return;
        }

        pool.Release(enemy);
    }

    public IEnumerator Prewarm(EnemyData enemyData, int count, int createPerFream = 20)
    {
        ObjectPool<EnemyRuntime> pool = GetOrCreatePool(enemyData);

        List<EnemyRuntime> prewarmList = new List<EnemyRuntime>(count);
        

        int createdCount = 0;

        while (createdCount < count)
        {
            int frameCount = Mathf.Min(createPerFream, count - createdCount);

            for (int i = 0; i < frameCount; i++)
            {
                EnemyRuntime enemy = pool.Get();
                
                prewarmList.Add(enemy);
            }

            createdCount += frameCount;
            yield return null;
        }

        foreach (EnemyRuntime enemy in prewarmList)
        {
            pool.Release(enemy);
        }
    }


    private ObjectPool<EnemyRuntime> GetOrCreatePool(
        EnemyData enemyData)
    {
        if (_pools.TryGetValue(
                enemyData.Id,
                out ObjectPool<EnemyRuntime> pool))
        {
            return pool;
        }

        pool = CreatePool(enemyData);

        _pools.Add(
            enemyData.Id,
            pool);

        return pool;
    }


    private ObjectPool<EnemyRuntime> CreatePool(
        EnemyData enemyData)
    {
        return new ObjectPool<EnemyRuntime>(
            createFunc: () =>
            {
                EnemyRuntime enemy =
                    Instantiate(
                        enemyData.Resource.Prefab,
                        transform);

                enemy.gameObject.SetActive(false);

                return enemy;
            },

            actionOnGet: null,

            actionOnRelease: enemy =>
            {
                enemy.gameObject.SetActive(false);

                enemy.transform.SetParent(transform);
            },

            actionOnDestroy: enemy =>
            {
                Destroy(enemy.gameObject);
            },

            collectionCheck: true,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize);
    }
}