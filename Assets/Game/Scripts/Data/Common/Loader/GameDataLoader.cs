using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameDataLoader : MonoBehaviour
{
    private IDataSource _dataSource;
    
    [TitleGroup("Data Source","Google Sheet")]
    [SerializeField]
    [Required]
    private string _apiKey;
    
    [TitleGroup("Data Source","Google Sheet")]
    [SerializeField]
    [Required]
    private string _spreadSheetId;

    [TitleGroup("Data Registry","Player Data")]
    [SerializeField]
    [Required]
    private PlayerResourceRegistry _playerRegistry;
    

    public void Initialize()
    {
        _dataSource = new GoogleSheetClient(_spreadSheetId, _apiKey);
    }

    public async Awaitable LoadDataAsync()
    {
        try
        {
            DataResponse response = await _dataSource.LoadAsync();
            ParsePlayerData(response.Content);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    private void ParsePlayerData(string json)
    {
        // SheetParser 
        List<PlayerDto> playerDtos = SheetParser.Parse<PlayerDto>(json, "PlayerData");


        // dto + resource => PlayerData (Runtime 생성 전 원본 데이터)
        PlayerBuilder builder = new PlayerBuilder(_playerRegistry);
        List<PlayerData> playerDataList = builder.Build(playerDtos);
        
        Dictionary<string, PlayerData> playerDataDic = new Dictionary<string, PlayerData>();

        foreach (PlayerData playerData in playerDataList)
        {
            playerDataDic.Add(playerData.Id, playerData);
        }
        
        GameDataStore.Instance.SetPlayerData(playerDataDic);
    }
}