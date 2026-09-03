
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField]
    private List<EnemyRuntime> _enemyList = new();

    public IReadOnlyList<EnemyRuntime> EnemyList => _enemyList;
}