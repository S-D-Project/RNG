using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Title("Runtime")]
    [ShowInInspector]
    [ReadOnly]
    public int CurrentWave { get; private set; }

    [ShowInInspector]
    [ReadOnly]
    public int CurrentWaveKillCount { get; private set; }

    [ShowInInspector]
    [ReadOnly]
    public float CurrentWaveElapsedTime { get; private set; }


    private EnemyManager _enemyManager;
    private EnemySpawner _enemySpawner;


    public void Initialize(
        EnemyManager enemyManager,
        EnemySpawner enemySpawner)
    {
        _enemyManager = enemyManager;
        _enemySpawner = enemySpawner;
    }


    public IEnumerator PlayStage(
        StageSpawnPlan stageSpawnPlan)
    {
        _enemyManager.EnemyKilled += OnEnemyKilled;

        try
        {
            foreach (WaveSpawnPlan wave
                     in stageSpawnPlan.WaveList)
            {
                yield return PlayWave(wave);
            }
        }
        finally
        {
            _enemyManager.EnemyKilled -= OnEnemyKilled;
        }
    }


    private IEnumerator PlayWave(
        WaveSpawnPlan wave)
    {
        CurrentWave = wave.WaveNumber;
        CurrentWaveKillCount = 0;
        CurrentWaveElapsedTime = 0f;

        List<SpawnGroupState> spawnStates =
            CreateSpawnStates(wave);


        while (true)
        {
            CurrentWaveElapsedTime +=
                Time.deltaTime;


            UpdateSpawnGroups(
                spawnStates,
                CurrentWaveElapsedTime);


            bool spawnCompleted =
                IsSpawnCompleted(spawnStates);


            if (spawnCompleted &&
                CanAdvanceWave(wave))
            {
                break;
            }


            yield return null;
        }
    }


    private List<SpawnGroupState> CreateSpawnStates(
        WaveSpawnPlan wave)
    {
        List<SpawnGroupState> states =
            new List<SpawnGroupState>(
                wave.SpawnGroups.Count);

        foreach (WaveSpawnGroupPlan group
                 in wave.SpawnGroups)
        {
            states.Add(
                new SpawnGroupState(group));
        }

        return states;
    }


    private void UpdateSpawnGroups(
        List<SpawnGroupState> states,
        float elapsedTime)
    {
        foreach (SpawnGroupState state in states)
        {
            if (state.IsComplete)
            {
                continue;
            }

            UpdateSpawnGroup(
                state,
                elapsedTime);
        }
    }


    private void UpdateSpawnGroup(
        SpawnGroupState state,
        float elapsedTime)
    {
        WaveSpawnGroupPlan plan =
            state.Plan;

        int totalCount =
            plan.EnemyList.Count;


        if (totalCount == 0)
        {
            return;
        }


        // SpawnDuration이 0 이하인 경우 즉시 전부 Spawn
        if (plan.SpawnDuration <= 0f)
        {
            SpawnUntil(
                state,
                totalCount);

            return;
        }


        float normalizedTime =
            Mathf.Clamp01(
                elapsedTime /
                plan.SpawnDuration);


        float spawnProgress =
            EvaluateSpawnProgress(
                plan.SpawnTiming,
                normalizedTime);


        int targetSpawnCount =
            Mathf.FloorToInt(
                totalCount *
                spawnProgress);


        // 부동소수점 오차와 관계없이
        // SpawnDuration 종료 시 모든 Enemy Spawn 보장
        if (normalizedTime >= 1f)
        {
            targetSpawnCount =
                totalCount;
        }


        SpawnUntil(
            state,
            targetSpawnCount);
    }


    private void SpawnUntil(
        SpawnGroupState state,
        int targetCount)
    {
        while (state.SpawnedCount < targetCount)
        {
            int spawnIndex =
                state.SpawnedCount;

            EnemyData enemyData =
                state.Plan.EnemyList[
                    spawnIndex];

            _enemySpawner.Spawn(
                enemyData);

            state.SpawnedCount++;
        }
    }


    private float EvaluateSpawnProgress(
        SpawnTimingType timing,
        float normalizedTime)
    {
        normalizedTime =
            Mathf.Clamp01(
                normalizedTime);


        switch (timing)
        {
            case SpawnTimingType.Constant:

                return normalizedTime;


            case SpawnTimingType.Accelerating:

                return normalizedTime *
                       normalizedTime;


            case SpawnTimingType.Decelerating:

                float inverse =
                    1f - normalizedTime;

                return 1f -
                       inverse * inverse;


            default:

                return normalizedTime;
        }
    }


    private bool IsSpawnCompleted(
        List<SpawnGroupState> states)
    {
        foreach (SpawnGroupState state in states)
        {
            if (!state.IsComplete)
            {
                return false;
            }
        }

        return true;
    }


    private bool CanAdvanceWave(
        WaveSpawnPlan wave)
    {
        bool reachedKillThreshold =
            wave.KillThreshold > 0 &&
            CurrentWaveKillCount >=
            wave.KillThreshold;


        bool reachedTimeLimit =
            wave.MaxDuration > 0f &&
            CurrentWaveElapsedTime >=
            wave.MaxDuration;


        return reachedKillThreshold ||
               reachedTimeLimit;
    }


    private void OnEnemyKilled(
        EnemyRuntime enemy)
    {
        CurrentWaveKillCount++;
    }


    private class SpawnGroupState
    {
        public WaveSpawnGroupPlan Plan { get; }

        public int SpawnedCount { get; set; }
        
        public bool IsComplete =>
            SpawnedCount >=
            Plan.EnemyList.Count;


        public SpawnGroupState(
            WaveSpawnGroupPlan plan)
        {
            Plan = plan;
            SpawnedCount = 0;
        }
    }
}