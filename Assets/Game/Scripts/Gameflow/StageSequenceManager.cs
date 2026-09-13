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

        yield return null;
    }

    private IEnumerator PrepareStage()
    {
        // Enemy Pool Prewarm

        yield return null;
    }

    private IEnumerator ReadySequence()
    {
        // TODO
        // Ready UI
        // 3 ,2 ,1 카운트다운 등
        yield return null;
    }

    private IEnumerator PlayStage()
    {
        // TODO
        // WaveManager.Start()
        
        yield return null;
    }

    private IEnumerator ClearSequence()
    {
        // TODO
        // Stage Clear

        yield return null;
    }
}