using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemBuilder
{
    private readonly ItemResourceRegistry _itemRegistry;

    public ItemBuilder(ItemResourceRegistry itemRegistry)
    {
        _itemRegistry = itemRegistry;
    }

    public List<ItemData> Build(IReadOnlyList<ItemDto> itemDtos, IReadOnlyList<ItemEffectDto> effectDtos)
    {
        var result = new List<ItemData>();

        foreach (var dto in itemDtos)
        {
            ItemResource resource = _itemRegistry.Find(dto.Id);
            
            if(resource == null)
            {
                Debug.LogWarning($"EnemyResource not found : {dto.Id}");
                continue;
            }

            List<ItemEffectData> effects = effectDtos.Where(x => x.ItemId == dto.Id)
                .Select(x => new ItemEffectData(x))
                .ToList();
            
            result.Add(new ItemData(resource,dto,effects));
        }

        return result;
    }
}