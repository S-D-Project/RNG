using System.Collections.Generic;
using UnityEngine;

/**
 * 인게임에서 사용할 Runtime 객체
 */
public class PlayerRuntime : MonoBehaviour
{
    private MovementSystem _movementSystem;
    public PlayerData BaseData { get; private set; }

    private readonly List<WeaponRuntime> _weapons = new();
    public IReadOnlyList<WeaponRuntime> Weapons => _weapons;

    public string Id { get; private set; }
    public float MaxHp { get; private set; }
    public float MoveSpeed { get; private set; }
    
    public int Level { get; private set; }
    public float Cooldown { get; private set; }
    
    private bool _isInitialized = false;
    
    public void Initialize(PlayerData baseData)
    {
        if (_isInitialized)
        {
            return;
        }
        BaseData = baseData;
        Id = baseData.Id;
        MaxHp = baseData.MaxHp;
        MoveSpeed = baseData.MoveSpeed;
        Level = 1;
        
        _movementSystem = gameObject.GetComponent<MovementSystem>();
        _movementSystem.SetMoveSpeed(MoveSpeed);

        _isInitialized = true;
    }

    public void AddWeapon(WeaponRuntime weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("Weapon is null");
            return;
        }
        _weapons.Add(weapon);
    }

    private void SetCooldown(float amount)
    {
        Cooldown = amount;
    }
}