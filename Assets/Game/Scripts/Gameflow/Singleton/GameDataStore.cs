using System.Collections.Generic;
using Sirenix.OdinInspector;

public class GameDataStore : Singleton<GameDataStore>
{
    [ShowInInspector]
    [ReadOnly]
    private Dictionary<string, PlayerData> _playerDataDic;

    [ShowInInspector]
    [ReadOnly]
    private Dictionary<string, WeaponData> _weaponDataDic;


    [ShowInInspector]
    [ReadOnly]
    private Dictionary<string, EnemyData> _enemyDataDic;
    
    public IReadOnlyDictionary<string, PlayerData> PlayerDataDic => _playerDataDic;
    public IReadOnlyDictionary<string, WeaponData> WeaponDataDic => _weaponDataDic;
    public IReadOnlyDictionary<string, EnemyData> EnemyDataDic => _enemyDataDic;
    
    public override void OnInitialize()
    {
        _playerDataDic = new Dictionary<string, PlayerData>();
        
    }

    public void SetPlayerData(Dictionary<string, PlayerData> playerDataDic)
    {
        _playerDataDic = playerDataDic;
    }

    public PlayerData GetPlayerData(string id)
    {
        return _playerDataDic.TryGetValue(id, out PlayerData data) ? data : null;
    }

    public void SetWeaponData(Dictionary<string, WeaponData> weaponDataDic)
    {
        _weaponDataDic = weaponDataDic;
    }

    public void SetEnemyData(Dictionary<string, EnemyData> enemyDataDic)
    {
        _enemyDataDic = enemyDataDic;
    }
    
    public WeaponData GetWeaponData(string id)
    {
        return _weaponDataDic.TryGetValue(id, out WeaponData data) ? data : null;
    }

    public EnemyData GetEnemyData(string id)
    {
        return _enemyDataDic.TryGetValue(id, out EnemyData data) ? data : null;
    }
    
}