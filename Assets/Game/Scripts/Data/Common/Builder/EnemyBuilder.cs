using System.Collections.Generic;
using UnityEngine;

public class EnemyBuilder
{
    private readonly EnemyResourceRegistry _enemyRegistry;

    public EnemyBuilder(EnemyResourceRegistry enemyRegistry)
    {
        _enemyRegistry = enemyRegistry;
    }

    public List<EnemyData> Build(IReadOnlyList<EnemyDto> dtos)
    {
        var result = new List<EnemyData>();

        foreach (var dto in dtos)
        {
            EnemyResource resource = _enemyRegistry.Find(dto.Id);
            
            if(resource == null)
            {
                Debug.LogWarning($"EnemyResource not found : {dto.Id}");
                continue;
            }

            result.Add(new EnemyData(dto, resource));
            
        }

        return result;
    }
}