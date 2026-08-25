using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilder
{
    private readonly PlayerResourceRegistry _playerRegistry;

    public PlayerBuilder(PlayerResourceRegistry playerRegistry)
    {
        _playerRegistry = playerRegistry;
    }

    public List<PlayerData> Build(IReadOnlyList<PlayerDto> dtos)
    {
        var result = new List<PlayerData>();

        foreach (var dto in dtos)
        {
            PlayerResource resource = _playerRegistry.Find(dto.Id);

            if (resource == null)
            {
                Debug.LogError($"PlayerResource not found : {dto.Id}");
                continue;
            }
            result.Add(new PlayerData(dto,resource));
        }

        return result;
    }
        
}