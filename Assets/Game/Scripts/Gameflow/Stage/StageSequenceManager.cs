using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageSequenceManager : MonoBehaviour
{
    private StageRuntime _stageRuntime;
    private EnemyManager _enemyManager;

    private EnemyPool _enemyPool;
    private WaveManager _waveManager;

    [ShowInInspector]
    public int TotalKillCount => _stageRuntime.TotalKillCount;

    private IReadOnlyList<WaveData>
        _waveDataList;

    private StageSpawnPlan _stageSpawnPlan;


    public void Initialize(
        StageRuntime stageRuntime,
        EnemyPool enemyPool,
        WaveManager waveManager,
        EnemyManager enemyManager)
    {
        _stageRuntime = stageRuntime;
        _enemyPool = enemyPool;
        _waveManager = waveManager;
        _enemyManager = enemyManager;

        _waveDataList = _stageRuntime.Definition.Waves;
    }


    public IEnumerator StartSequence()
    {
        _enemyManager.EnemyKilled += OnEnemyKilled;

        yield return PrepareStage();

        yield return ReadyStage();

        yield return PlayStage();

        yield return ClearStage();

        //_enemyManager.EnemyKilled -= OnEnemyKilled;
    }


    private IEnumerator PrepareStage()
    {
        Debug.Log("PrepareStage");
        WaveSpawnPlanBuilder builder =
            new();

        _stageSpawnPlan =
            builder.Build(
                _waveDataList);


        Dictionary<string, EnemyPrewarmData>
            prewarmDataDic =
                _stageSpawnPlan
                    .CalculatePrewarmData();


        foreach (EnemyPrewarmData prewarmData
                 in prewarmDataDic.Values)
        {
            yield return _enemyPool.Prewarm(
                prewarmData.EnemyData,
                prewarmData.Count);
        }
    }


    private IEnumerator ReadyStage()
    {
        // TODO
        // Ready UI
        // 3...2...1... Countdown

        yield return null;
    }


    private IEnumerator PlayStage()
    {
        Debug.Log("Play Stage");
        yield return _waveManager.PlayStage(
            _stageSpawnPlan);

        yield return WaitForRemainingEnemies();
    }


    private IEnumerator ClearStage()
    {
        // TODO
        // Stage Clear UI
        // Reward
        Debug.Log("Stage Clear");
        yield return null;
    }

    private IEnumerator WaitForRemainingEnemies()
    {
        while (_enemyManager.ActiveEnemyCount > 0)
        {
            yield return null;
        }
    }

    private void OnEnemyKilled(EnemyRuntime enemy)
    {
        _stageRuntime.AddKill();
    }
}