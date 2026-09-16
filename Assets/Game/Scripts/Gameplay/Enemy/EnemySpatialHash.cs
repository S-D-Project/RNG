using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpatialHash : IEnemySpatialQuery
{
    private readonly float _cellSize;

    private readonly Dictionary<Vector2Int, HashSet<EnemyRuntime>> _cells = new();
    private readonly Dictionary<EnemyRuntime, Vector2Int> _enemyCells = new();
    private float _maxHitRadius;

    public EnemySpatialHash(float cellSize)
    {
        if (cellSize <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(cellSize));
        }

        _cellSize = cellSize;
    }

    public void Register(EnemyRuntime enemy)
    {
        Vector2Int cellPosition = GetCellPosition(enemy.transform.position);

        if (!_cells.TryGetValue(cellPosition, out HashSet<EnemyRuntime> cell))
        {
            cell = new HashSet<EnemyRuntime>();
            _cells.Add(cellPosition, cell);
        }

        cell.Add(enemy);
        _enemyCells.Add(enemy, cellPosition);

        if (enemy.HitRadius > _maxHitRadius)
        {
            _maxHitRadius = enemy.HitRadius;
        }
    }

    public void UnRegister(EnemyRuntime enemy)
    {
        // TODO 실제 Enemy 제거 시 연결 해야 됨. >> Enemy Death / Pooling 시스템 만들 때 연결
        
        bool recalculateMaxRadius = enemy.HitRadius >= _maxHitRadius;

        Vector2Int cellPosition = _enemyCells[enemy];

        HashSet<EnemyRuntime> cell = _cells[cellPosition];

        cell.Remove(enemy);
        _enemyCells.Remove(enemy);

        if (cell.Count == 0)
        {
            _cells.Remove(cellPosition);
        }

        if (recalculateMaxRadius)
        {
            RecalculateMaxHitRadius();
        }
    }

    public void UpdatePosition(EnemyRuntime enemy)
    {
        Vector2Int previousCell = _enemyCells[enemy];

        Vector2Int currentCell = GetCellPosition(enemy.transform.position);

        if (previousCell == currentCell)
        {
            return;
        }

        MoveCell(enemy, previousCell, currentCell);
    }

    public Vector2Int GetCellPosition(Vector3 position)
    {
        return new Vector2Int(Mathf.FloorToInt(position.x / _cellSize), Mathf.FloorToInt(position.y / _cellSize));
    }

    private void MoveCell(EnemyRuntime enemy, Vector2Int previousCellPosition, Vector2Int currentCellPositioin)
    {
        HashSet<EnemyRuntime> previousCell = _cells[previousCellPosition];
        previousCell.Remove(enemy);

        if (previousCell.Count == 0)
        {
            _cells.Remove(previousCellPosition);
        }

        if (!_cells.TryGetValue(currentCellPositioin, out HashSet<EnemyRuntime> currentCell))
        {
            currentCell = new HashSet<EnemyRuntime>();

            _cells.Add(currentCellPositioin, currentCell);
        }

        currentCell.Add(enemy);

        _enemyCells[enemy] = currentCellPositioin;
    }

    // 타겟팅
    public void Query(Vector2 center, float radius, List<EnemyRuntime> results)
    {
        results.Clear();

        Vector2 min = center - Vector2.one * radius;
        Vector2 max = center + Vector2.one * radius;

        Vector2Int minCell = GetCellPosition(min);
        Vector2Int maxCell = GetCellPosition(max);

        float sqrRadius = radius * radius;

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector2Int cellPosition = new Vector2Int(x, y);

                if (!_cells.TryGetValue(cellPosition, out HashSet<EnemyRuntime> cell))
                {
                    continue;
                }

                foreach (EnemyRuntime enemy in cell)
                {
                    if (enemy.IsDead)
                    {
                        continue;
                    }
                    Vector2 enemyPosition = enemy.transform.position;

                    float sqrDistance = (enemyPosition - center).sqrMagnitude;

                    if (sqrDistance > sqrRadius)
                    {
                        continue;
                    }

                    results.Add(enemy);
                }
            }
        }
    }

    // 충돌 검사용
    public void QueryOverlapCircle(
        Vector2 center,
        float radius,
        List<EnemyRuntime> results)
    {
        results.Clear();

        float searchRadius =
            radius + _maxHitRadius;

        Vector2 min =
            center - Vector2.one * searchRadius;

        Vector2 max =
            center + Vector2.one * searchRadius;

        Vector2Int minCell =
            GetCellPosition(min);

        Vector2Int maxCell =
            GetCellPosition(max);

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector2Int cellPosition =
                    new Vector2Int(x, y);

                if (!_cells.TryGetValue(
                        cellPosition,
                        out HashSet<EnemyRuntime> cell))
                {
                    continue;
                }

                foreach (EnemyRuntime enemy in cell)
                {
                    if (enemy.IsDead)
                    {
                        continue;
                    }
                    
                    Vector2 enemyPosition =
                        enemy.transform.position;

                    float collisionRadius =
                        radius + enemy.HitRadius;

                    float sqrDistance =
                        (enemyPosition - center)
                        .sqrMagnitude;

                    if (sqrDistance >
                        collisionRadius * collisionRadius)
                    {
                        continue;
                    }

                    results.Add(enemy);
                }
            }
        }
    }

    private void RecalculateMaxHitRadius()
    {
        _maxHitRadius = 0f;
        foreach (EnemyRuntime enemy in _enemyCells.Keys)
        {
            if (enemy.HitRadius > _maxHitRadius)
            {
                _maxHitRadius = enemy.HitRadius;
            }
        }
    }
}