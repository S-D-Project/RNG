using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnPlanBuilder
{
    public StageSpawnPlan Build(
        IReadOnlyList<WaveData> waveDataList)
    {
        List<WaveSpawnPlan> wavePlanList =
            new List<WaveSpawnPlan>();

        foreach (WaveData waveData in waveDataList)
        {
            wavePlanList.Add(
                BuildWave(waveData));
        }

        return new StageSpawnPlan(
            wavePlanList);
    }


    private WaveSpawnPlan BuildWave(
        WaveData waveData)
    {
        List<WaveSpawnGroupPlan> spawnGroups =
            new List<WaveSpawnGroupPlan>();

        foreach (WaveSpawnData spawnData
                 in waveData.SpawnList)
        {
            if (spawnData.SpawnDuration >
                waveData.MaxDuration)
            {
                Debug.LogError(
                    $"SpawnDuration must be <= MaxDuration. " +
                    $"Wave: {waveData.WaveNumber}");

                continue;
            }

            List<EnemyData> enemyList =
                BuildEnemyList(spawnData);

            WaveSpawnGroupPlan groupPlan =
                new WaveSpawnGroupPlan(
                    enemyList,
                    spawnData.SpawnDuration,
                    spawnData.SpawnTiming);

            spawnGroups.Add(groupPlan);
        }

        return new WaveSpawnPlan(
            waveData.WaveNumber,
            waveData.KillThreshold,
            waveData.MaxDuration,
            spawnGroups);
    }


    private List<EnemyData> BuildEnemyList(
        WaveSpawnData spawnData)
    {
        List<EnemyData> enemyList =
            new List<EnemyData>();

        switch (spawnData.SpawnType)
        {
            case EnemySpawnType.Fixed:

                AddFixedEnemy(
                    spawnData,
                    enemyList);

                break;


            case EnemySpawnType.RandomGrade:

                AddRandomGradeEnemy(
                    spawnData,
                    enemyList);

                break;
        }

        return enemyList;
    }


    private void AddFixedEnemy(
        WaveSpawnData spawnData,
        List<EnemyData> enemyList)
    {
        EnemyData enemyData =
            GameDataStore.Instance
                .GetEnemyData(
                    spawnData.EnemyId);

        if (enemyData == null)
        {
            Debug.LogError(
                $"EnemyData not found. Id: {spawnData.EnemyId}");

            return;
        }

        for (int i = 0; i < spawnData.Count; i++)
        {
            enemyList.Add(enemyData);
        }
    }


    private void AddRandomGradeEnemy(
        WaveSpawnData spawnData,
        List<EnemyData> enemyList)
    {
        IReadOnlyList<EnemyData> candidates =
            GameDataStore.Instance
                .GetEnemyDataByGrade(
                    spawnData.Grade);

        if (candidates == null ||
            candidates.Count == 0)
        {
            Debug.LogError(
                $"EnemyData not found. Grade: {spawnData.Grade}");

            return;
        }

        for (int i = 0; i < spawnData.Count; i++)
        {
            int index =
                Random.Range(
                    0,
                    candidates.Count);

            enemyList.Add(
                candidates[index]);
        }
    }
}