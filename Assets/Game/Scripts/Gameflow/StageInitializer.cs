
using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageInitializer : MonoBehaviour
{
    [Title("Spawner")]
    [SerializeField]
    [Required]
    private PlayerSpawner _playerSpawner;
    
    [Title("AttackRuntimeManager")]
    [SerializeField] 
    [Required] 
    private AttackRuntimeManager _attackRuntimeManager;

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

        // TODO 임시로 Weapon 추가. 나중에 분리해야 함.
        WeaponData weaponData = GameDataStore.Instance.GetWeaponData("bullet");
        WeaponRuntime weaponRuntime = new WeaponRuntime(weaponData);
        player.AddWeapon(weaponRuntime);
        
        // TODO 실제 무기 생성 
        GameObject weaponObjectPrefab = weaponData.WeaponObjectPrefab;
        GameObject weapon = Instantiate(weaponObjectPrefab, player.transform);
        WeaponController weaponController = weapon.GetComponent<WeaponController>();
        weaponController.Initialize(weaponRuntime,_attackRuntimeManager);


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