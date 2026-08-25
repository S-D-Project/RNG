
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
        await InitializeAsync();
    }

    private async Awaitable InitializeAsync()
    {
        await _gameDataLoader.LoadDataAsync();
    }
    
}