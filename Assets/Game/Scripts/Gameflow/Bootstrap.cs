
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Title("Initialize")]
    [SerializeField]
    [Required]
    private GameDataLoader _gameDataLoader;
    
    private bool IsInitialized = false;
    
    private async void Start()
    {
        InitializeSingleton();
        
        await InitializeAsync();

        IsInitialized = true;

        // TODO 나중에 Game Start 버튼으로 기능 옮길 것. 일단 임시로 Game Start
        await SceneFlowManager.Instance.EnterGame();
    }

    private void InitializeSingleton()
    {
        GameDataStore.Instance.OnInitialize();
        SceneFlowManager.Instance.OnInitialize();
    }

    private async Awaitable InitializeAsync()
    {
        _gameDataLoader.Initialize();
        await _gameDataLoader.LoadDataAsync();
    }
    
}