using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageSequenceManager : MonoBehaviour
{
 
    [SerializeField]
    private Sprite _icon1;
    [SerializeField]
    private Sprite _icon2;
    [SerializeField]
    private Sprite _icon3;
    
    private StageRuntime _stageRuntime;
    private EnemyManager _enemyManager;

    private EnemyPool _enemyPool;
    private WaveManager _waveManager;
    
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
        
        yield return new WaitForSeconds(2f);

        LevelUpPopup popup = 
            PopupManager.Instance.Open<LevelUpPopup>(PopupType.LevelUp);
        
        IReadOnlyList<LevelUpOption> option = TestCreateLevelUpOptions();
            
        popup.SetData(3,option,TestOnLevelUpComplted);

        
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

    private IReadOnlyList<LevelUpOption> TestCreateLevelUpOptions()
    {
        List<LevelUpOption> options = new();

        LevelUpOption option1 = new LevelUpOption(
            _icon1, "Weapon", "Bullet", "스피드 +10%", "Lv.1", TestLevelUpOption1);

        LevelUpOption option2 = new LevelUpOption(
            _icon2, "Item", "Appl2", "체력 +10%", "Lv.3", TestLevelUpOption2);
        LevelUpOption option3 = new LevelUpOption(
            _icon3, "Item", "Apple3", "사거리 +5%", "Lv.5", TestLevelUpOption3);
        
        options.Add(option1);
        options.Add(option2);
        options.Add(option3);

        return options;
    }

    private void TestOnLevelUpComplted()
    {
        Debug.Log("TestOnLevelUpComplted");
    }

    private void TestLevelUpOption1()
    {
        PlayerManagerSingleton.Instance.AddWeapon("bullet");
    }
    private void TestLevelUpOption2()
    {
        Debug.Log("Hp Up");
    }
    private void TestLevelUpOption3()
    {
        Debug.Log("Range Up");
    }
}