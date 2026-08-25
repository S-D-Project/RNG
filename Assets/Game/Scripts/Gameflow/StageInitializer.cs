
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageInitializer : MonoBehaviour
{
    [Title("Required Settings")]
    [SerializeField]
    [Required]
    private PlayerSpawner _playerSpawner;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        InitializeStage();
        InitializePlayer();
        InitializeUI();
        
        StartGame();
    }

    private void InitializeStage()
    {
        // TODO stage 초기화
    }

    private void InitializePlayer()
    {
        _playerSpawner.SpawnPlayer();
    }

    private void InitializeUI()
    {
        // TODO UI 초기화
    }

    private void StartGame()
    {
        // TODO Game 시작 
    }
}