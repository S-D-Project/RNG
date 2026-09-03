
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField]
    private List<EnemyRuntime> _enemyList = new();

    // TODO _enemyList를 참조하는 대상은 대부분 공간 분할 리팩토링 대상
    public IReadOnlyList<EnemyRuntime> EnemyList => _enemyList;
}