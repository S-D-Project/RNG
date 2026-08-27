
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageInitializer : MonoBehaviour
{
    [Title("Spawner")]
    [SerializeField]
    [Required]
    private PlayerSpawner _playerSpawner;

    [Title("Player Prefab")]
    [SerializeField]
    [Required]
    [InfoBox("지금은 인스펙터 창에서 넣는데 나중에 캐릭터 선택 만들면 대체할 예정")]
    private string _selectedCharacterId;

    private void Start()
    {
        Initialize();
    }

    /**
     * TODO 지금은 인스펙터 창에서 캐릭터를 넣어두는데, 나중에 캐릭터 선택 상호작용과 연결 
     */
    public void SetCharacter(string id)
    {
        _selectedCharacterId = id;
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
        PlayerRuntime player = _playerSpawner.SpawnPlayer(Vector2.zero,_selectedCharacterId);

        WeaponData weaponData = GameDataStore.Instance.GetWeaponData("bullet");
        WeaponRuntime weaponRuntime = new WeaponRuntime(weaponData);
        
        player.AddWeapon(weaponRuntime);
        Debug.Log($"Weapon Count : {player.Weapons.Count}");
        Debug.Log($"Weapon Id : {player.Weapons[0].BaseData.Id}");
    }

    private void InitializeUI()
    {
        // TODO UI 초기화
    }

    private void StartGame()
    {
        // TODO Game시작 로직 
    }
}