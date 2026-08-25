using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameDataLoader : MonoBehaviour
{
    [SerializeField] [Required] private IDataSource _dataSource;

    [SerializeField] [Required] private PlayerResourceRegistry _playerRegistry;

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
        
        // TODO 0825 -> GameDataStore생성 후, playerDataList 저장
    }
}