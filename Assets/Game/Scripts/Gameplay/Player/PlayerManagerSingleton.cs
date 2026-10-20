using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerManagerSingleton : Singleton<PlayerManagerSingleton>
{
    private PlayerRuntime _playerRuntime;
    private PlayerWeaponControllerManager _playerWeaponControllerManager;
    private AttackRuntimeManager _attackRuntimeManager;

    public PlayerItemInventory ItemInventory { get; private set; }

    public void Initialize(PlayerRuntime playerRuntime, AttackRuntimeManager attackRuntimeManager)
    {
        _playerRuntime = playerRuntime;
        _playerWeaponControllerManager = _playerRuntime.gameObject.GetComponent<PlayerWeaponControllerManager>();
        _attackRuntimeManager = attackRuntimeManager;

        ItemInventory = new PlayerItemInventory(6);
    }

    public WeaponRuntime AddWeapon(string weaponId)
    {
        WeaponData weaponData = GameDataStore.Instance.GetWeaponData(weaponId);

        Debug.Log($"WeaponTag : {weaponData.WeaponTags}");

        WeaponRuntime weaponRuntime = new WeaponRuntime(weaponData);

        _playerRuntime.AddWeapon(weaponRuntime);

        GameObject weapon = Instantiate(weaponRuntime.BaseData.WeaponObjectPrefab, _playerRuntime.transform);
        WeaponController weaponController = weapon.GetComponent<WeaponController>();

        weaponController.Initialize(weaponRuntime, _attackRuntimeManager);

        _playerWeaponControllerManager.AddWeapon(weaponRuntime, weaponController);

        return weaponRuntime;
    }

    [Button]
    public void TestInventoryTryAcquire()
    {
        ItemData potato =
            GameDataStore.Instance.GetItemData("potato");

        IReadOnlyList<WeaponRuntime> weaponRuntimes = _playerWeaponControllerManager.GetWeaponRuntimes();

        List<ItemInventoryAcquireResult> results = new List<ItemInventoryAcquireResult>();

        int i = 1;
        foreach (var weaponRuntime in weaponRuntimes)
        {
            var result = ItemInventory.TryAcquire(potato, weaponRuntime);
            results.Add(result);
            Debug.Log($"Result{i++} : {result}");
        }
        
        foreach (OwnedItemEntry entry in ItemInventory.OwnedItems)
        {
            Debug.Log($"Item : {entry.Item.Id}, " + $"Level : {entry.Item.Level}");
        }
    }
}