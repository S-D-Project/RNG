using System.Collections;
using UnityEngine;

public class StageSequenceManager : MonoBehaviour
{
    private EnemyPool _enemyPool;
    private WaveManager _waveManager;

    public void Initialize(EnemyPool enemyPool, WaveManager waveManager)
    {
        _enemyPool = enemyPool;
        _waveManager = waveManager;
    }

    public IEnumerator StartSequence()
    {
        yield return PrepareStage();

        yield return ReadyStage();

        StartStage();
    }


    private IEnumerator PrepareStage()
    {
        // TODO
        // Wave Spawn Plan 생성
        // Enemy Pool Prewarm

        yield return null;
    }


    private IEnumerator ReadyStage()
    {
        // TODO
        // Ready UI
        // Countdown

        yield return null;
    }


    private void StartStage()
    {
        // TODO
        // WaveManager 시작
    }
}