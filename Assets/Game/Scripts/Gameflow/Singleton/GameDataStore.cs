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

    [ShowInInspector]
    [ReadOnly]
    private Dictionary<EnemyGrade, List<EnemyData>> _enemyGradeDataDic;
    
    public IReadOnlyDictionary<string, PlayerData> PlayerDataDic => _playerDataDic;
    public IReadOnlyDictionary<string, WeaponData> WeaponDataDic => _weaponDataDic;
    public IReadOnlyDictionary<string, EnemyData> EnemyDataDic => _enemyDataDic;
    public IReadOnlyDictionary<EnemyGrade, List<EnemyData>> EnemyGradeDataDic => _enemyGradeDataDic;
    
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

        _enemyGradeDataDic = new Dictionary<EnemyGrade, List<EnemyData>>();

        foreach (EnemyData enemyData in enemyDataDic.Values)
        {
            if (!_enemyGradeDataDic.TryGetValue(enemyData.Grade, out List<EnemyData> enemyList))
            {
                enemyList = new List<EnemyData>();
                _enemyGradeDataDic.Add(enemyData.Grade,enemyList);
            }
            enemyList.Add(enemyData);
        }
    }
    
    public WeaponData GetWeaponData(string id)
    {
        return _weaponDataDic.TryGetValue(id, out WeaponData data) ? data : null;
    }

    public EnemyData GetEnemyData(string id)
    {
        return _enemyDataDic.TryGetValue(id, out EnemyData data) ? data : null;
    }

    public IReadOnlyList<EnemyData> GetEnemyDataByGrade(EnemyGrade grade)
    {
        return _enemyGradeDataDic.TryGetValue(grade, out List<EnemyData> enemyList) ? enemyList : null;
    }
    
}