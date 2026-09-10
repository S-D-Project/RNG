
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public IEnemySpatialQuery SpatialQuery => _spatialHash;
    
    private float _spatialCellSize = 5f;
    private readonly List<EnemyRuntime> _enemyList = new();

    private EnemySpatialHash _spatialHash;
    private Transform _target;

    private float _maxHitRadius;
    public float MaxHitRadius => _maxHitRadius;
    
    public void Register(EnemyRuntime enemy)
    {
        _enemyList.Add(enemy);
        _spatialHash.Register(enemy);

        if (enemy.HitRadius > _maxHitRadius)
        {
            _maxHitRadius = enemy.HitRadius;
        }
    }

    public void UnRegister(EnemyRuntime enemy)
    {
        _spatialHash.UnRegister(enemy);
        _enemyList.Remove(enemy);
    }
    
    public void Initialize(Transform target)
    {
        _spatialHash = new EnemySpatialHash(_spatialCellSize);
        _target = target;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        
        UpdateEnemies(deltaTime);
    }

    private void UpdateEnemies(float deltaTime)
    {
        foreach (EnemyRuntime enemy in _enemyList)
        {
            MoveEnemy(enemy, deltaTime);
            
            _spatialHash.UpdatePosition(enemy);
        }
    }

    private void MoveEnemy(EnemyRuntime enemy, float deltaTime)
    {

        Vector2 position = enemy.transform.position;

        Vector2 targetPosition = _target.position;

        Vector2 direction = (targetPosition - position).normalized;

        enemy.transform.position += (Vector3) direction * (enemy.MoveSpeed * deltaTime);
    }
}