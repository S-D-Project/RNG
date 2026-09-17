using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

public class EnemyManager : Singleton<EnemyManager>
{
    [Title("Player")]
    [SerializeField]
    [ReadOnly]
    private Transform _playerTransform;

    private PlayerRuntime _playerRuntime;
    private float _playerCollisionRadius;


    [Title("Spatial")]
    [SerializeField]
    private float _spatialCellSize = 5f;


    private EnemyPool _enemyPool;

    private EnemySpatialHash _spatialHash;

    private readonly List<EnemyRuntime> _enemyList = new();


    public int ActiveEnemyCount => _enemyList.Count;
    public event Action<EnemyRuntime> EnemyKilled;
    
    public IEnemySpatialQuery SpatialQuery => _spatialHash;


    public void Initialize(
        PlayerRuntime playerRuntime,
        EnemyPool enemyPool)
    {
        _playerRuntime = playerRuntime;
        _playerTransform = playerRuntime.transform;
        _playerCollisionRadius = playerRuntime.HitRadius;

        _enemyPool = enemyPool;

        _spatialHash =
            new EnemySpatialHash(_spatialCellSize);
    }


    public void Register(EnemyRuntime enemy)
    {
        _enemyList.Add(enemy);

        _spatialHash.Register(enemy);
    }


    public void UnRegister(EnemyRuntime enemy)
    {
        _spatialHash.UnRegister(enemy);

        _enemyList.Remove(enemy);
    }


    private void Update()
    {
        UpdateEnemies(Time.deltaTime);

        RemoveDeadEnemies();
    }


    private void UpdateEnemies(float deltaTime)
    {
        Vector2 playerPosition =
            _playerTransform.position;

        foreach (EnemyRuntime enemy in _enemyList)
        {
            if (enemy.IsDead)
            {
                continue;
            }

            UpdateEnemy(
                enemy,
                playerPosition,
                deltaTime);

            _spatialHash.UpdatePosition(enemy);
        }
    }


    private void UpdateEnemy(
        EnemyRuntime enemy,
        Vector2 playerPosition,
        float deltaTime)
    {
        Vector2 enemyPosition =
            enemy.transform.position;

        Vector2 toPlayer =
            playerPosition - enemyPosition;

        Vector2 direction =
            toPlayer.normalized;

        enemyPosition +=
            direction *
            (enemy.MoveSpeed * deltaTime);

        enemy.transform.position =
            enemyPosition;


        CheckPlayerCollision(
            enemy,
            enemyPosition,
            playerPosition);
    }


    private void CheckPlayerCollision(
        EnemyRuntime enemy,
        Vector2 enemyPosition,
        Vector2 playerPosition)
    {
        float collisionRadius =
            enemy.HitRadius +
            _playerCollisionRadius;

        float sqrDistance =
            (playerPosition - enemyPosition)
            .sqrMagnitude;

        if (sqrDistance >
            collisionRadius * collisionRadius)
        {
            return;
        }

        _playerRuntime.TakeDamage(
            enemy.ContactDamage);
    }


    private void RemoveDeadEnemies()
    {
        for (int i = _enemyList.Count - 1; i >= 0; i--)
        {
            EnemyRuntime enemy =
                _enemyList[i];

            if (!enemy.IsDead)
            {
                continue;
            }

            RemoveEnemy(
                enemy,
                i);
        }
    }


    private void RemoveEnemy(
        EnemyRuntime enemy,
        int index)
    {
        _spatialHash.UnRegister(enemy);

        _enemyList.RemoveAt(index);
        
        EnemyKilled?.Invoke(enemy);

        _enemyPool.Release(enemy);
    }
}