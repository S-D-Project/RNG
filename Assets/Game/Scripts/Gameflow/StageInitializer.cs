
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

        WeaponData weaponData2 = GameDataStore.Instance.GetWeaponData("fire_ball");
        WeaponRuntime weaponRuntime2 = new WeaponRuntime(weaponData2);
        player.AddWeapon(weaponRuntime2);

        WeaponData weapondata3 = GameDataStore.Instance.GetWeaponData("plasma_bullet");
        WeaponRuntime weaponRuntime3 = new WeaponRuntime(weapondata3);
        player.AddWeapon(weaponRuntime3);
        
        // TODO 실제 무기 생성 
        AddWeaponToPlayer(weaponData,weaponRuntime,player.gameObject);
        AddWeaponToPlayer(weaponData2,weaponRuntime2,player.gameObject);
        AddWeaponToPlayer(weapondata3,weaponRuntime3,player.gameObject);

    }
    
    private void AddWeaponToPlayer(WeaponData weaponData,WeaponRuntime runtime ,GameObject player)
    {
        GameObject weaponObjectPrefab = weaponData.WeaponObjectPrefab;
        GameObject weapon = Instantiate(weaponObjectPrefab, player.transform);
        WeaponController weaponController = weapon.GetComponent<WeaponController>();
        weaponController.Initialize(runtime, _attackRuntimeManager);
        PlayerWeaponControllerManager weaponControllerManager = player.GetComponent<PlayerWeaponControllerManager>();
        weaponControllerManager.AddWeapon(runtime,weaponController);

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