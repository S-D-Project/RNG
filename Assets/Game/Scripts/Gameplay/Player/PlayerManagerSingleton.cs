using UnityEngine;

public class PlayerManagerSingleton : Singleton<PlayerManagerSingleton>
{
    private PlayerRuntime _playerRuntime;
    private PlayerWeaponControllerManager _playerWeaponControllerManager;
    private AttackRuntimeManager _attackRuntimeManager;
    
    public void Initialize(PlayerRuntime  playerRuntime,AttackRuntimeManager attackRuntimeManager)
    {
        _playerRuntime = playerRuntime;
        _playerWeaponControllerManager = _playerRuntime.gameObject.GetComponent<PlayerWeaponControllerManager>();
        _attackRuntimeManager = attackRuntimeManager;
    }

    public void AddWeapon(string weaponId)
    {
        WeaponData weaponData = GameDataStore.Instance.GetWeaponData(weaponId);

        WeaponRuntime weaponRuntime = new WeaponRuntime(weaponData);
        
        _playerRuntime.AddWeapon(weaponRuntime);

        GameObject weapon = Instantiate(weaponRuntime.BaseData.WeaponObjectPrefab, _playerRuntime.transform);
        WeaponController weaponController = weapon.GetComponent<WeaponController>();
        
        weaponController.Initialize(weaponRuntime,_attackRuntimeManager);
        
        _playerWeaponControllerManager.AddWeapon(weaponRuntime,weaponController);

    }

    public void SelectLevelUpOption(int optionId)
    {
        
    }
    
}