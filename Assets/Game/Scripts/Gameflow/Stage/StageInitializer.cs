using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageInitializer : MonoBehaviour
{

    [Title("Stage")]
    [SerializeField]
    [Required]
    private StageSequenceManager _stageSequenceManager;

    [SerializeField]
    [Required]
    private WaveManager _waveManager;

    [SerializeField]
    [Required]
    private StageDefinition _stageDefinition;

    [SerializeField]
    [Required]
    private PlayerCamera _playerCamera;
    
    [SerializeField]
    [Required]
    private InfiniteMapController _infiniteMapController;

    
    [Title("Spawner")]
    [SerializeField]
    [Required]
    private PlayerSpawner _playerSpawner;
    [Required]
    [SerializeField]
    private EnemySpawner _enemySpawner;
    
    [Required]
    [SerializeField]
    private EnemyPool _enemyPool;

    [Title("Attack Runtime Manager")]
    [SerializeField]
    [Required]
    private AttackRuntimeManager _attackRuntimeManager;




    [Title("Test Settings")]
    [SerializeField]
    [Required]
    [InfoBox("임시 설정. 추후 캐릭터 선택 UI로 대체")]
    private string _selectedCharacterId;

    [SerializeField]
    [Required]
    private EnemyTestSpawner _testSpawner;

    public int SpawnCount = 5;

    [SerializeField]
    [Required]
    [InfoBox("임시 설정")]
    private string _testEnemyId;
    
    private PlayerRuntime _playerRuntime;
    
    private void Start()
    {
        Initialize();
    }


    public void SetCharacter(string id)
    {
        _selectedCharacterId = id;
    }


    private void Initialize()
    {
        InitializeStage();

        InitializePlayer();

        InitializeEnemy();

        InitializeCombat();

        InitializeUI();

        InitializeSequence();

        StartGame();
    }




    private void InitializeStage()
    {
        // TODO
        // StageData 로드
        // Stage 환경 설정
        // Stage Spawn 정보 준비
    }


    private void InitializePlayer()
    {
        _playerRuntime =
            _playerSpawner.SpawnPlayer(
                Vector2.zero,
                _selectedCharacterId);

        InitializeTestWeapon();
    }


    private void InitializeEnemy()
    {
        EnemyManager.Instance.Initialize(
            _playerRuntime,_enemyPool);

        _enemySpawner.Initialize(_playerRuntime.transform,_enemyPool);
        
        _waveManager.Initialize(EnemyManager.Instance,_enemySpawner);
    }


    private void InitializeCombat()
    {
        _attackRuntimeManager.Initialize(
            EnemyManager.Instance.SpatialQuery);
    }


    private void InitializeUI()
    {
        // TODO UI 초기화
        _playerCamera.Initialize(_playerRuntime.transform);
        _infiniteMapController.Initialize(_playerRuntime.transform);
    }

    private void InitializeSequence()
    {
        StageRuntime runtime = new StageRuntime(_stageDefinition);
        _stageSequenceManager.Initialize(runtime,_enemyPool,_waveManager,EnemyManager.Instance);
        
    }

    private void StartGame()
    {
        StartCoroutine(_stageSequenceManager.StartSequence());
    }


    private void InitializeTestWeapon()
    {
        WeaponData weaponData =
            GameDataStore.Instance.GetWeaponData(
                "bullet");

        WeaponRuntime weaponRuntime =
            new WeaponRuntime(weaponData);

        _playerRuntime.AddWeapon(
            weaponRuntime);

        AddWeaponToPlayer(
            weaponData,
            weaponRuntime,
            _playerRuntime.gameObject);
    }


    private void AddWeaponToPlayer(
        WeaponData weaponData,
        WeaponRuntime runtime,
        GameObject player)
    {
        GameObject weapon =
            Instantiate(
                weaponData.WeaponObjectPrefab,
                player.transform);

        WeaponController weaponController =
            weapon.GetComponent<WeaponController>();

        weaponController.Initialize(
            runtime,
            _attackRuntimeManager);

        PlayerWeaponControllerManager
            weaponControllerManager =
                player.GetComponent<
                    PlayerWeaponControllerManager>();

        weaponControllerManager.AddWeapon(
            runtime,
            weaponController);
    }
}