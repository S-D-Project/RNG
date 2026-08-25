using System.Collections.Generic;
using Sirenix.OdinInspector;

public class GameDataStore : Singleton<GameDataStore>
{
    [ShowInInspector]
    [ReadOnly]
    private Dictionary<string, PlayerData> _playerDataDic;
    public IReadOnlyDictionary<string, PlayerData> PlayerDataDic => _playerDataDic;
    
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
}