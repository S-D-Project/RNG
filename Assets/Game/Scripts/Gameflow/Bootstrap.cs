
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Title("Initialize")]
    [SerializeField]
    [Required]
    private GameDataLoader _gameDataLoader;
    
    private async void Start()
    {
        InitializeSingleton();
        
        await InitializeAsync();
        
        await SceneFlowManager.Instance.LoadSceneAsync(SceneNames.TitleScene);
    }

    private void InitializeSingleton()
    {
        GameDataStore.Instance.OnInitialize();
        SceneFlowManager.Instance.OnInitialize();
        PopupManager.Instance.OnInitialize();
    }

    private async Awaitable InitializeAsync()
    {
        _gameDataLoader.Initialize();
        await _gameDataLoader.LoadDataAsync();
    }
    
}