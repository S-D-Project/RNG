using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ItemData
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    
    public Sprite Icon { get; }

    public IReadOnlyList<ItemEffectData> Effects { get; }

    public ItemData(ItemResource resource, ItemDto dto, IReadOnlyList<ItemEffectData> effectDatas)
    {
        Id = dto.Id;
        Name = dto.Name;
        Icon = resource.Icon;

        Effects = new List<ItemEffectData>(effectDatas).AsReadOnly();

        Description = CreateDescription(dto.Description, Effects);
    }

    public static string CreateDescription(
        string template,
        IReadOnlyList<ItemEffectData> effects)
    {
        if (string.IsNullOrEmpty(template))
            return string.Empty;

        if (effects == null)
            return template;

        string description = template;

        HashSet<string> usedKeys = new();

        foreach (ItemEffectData effect in effects)
        {
            if (string.IsNullOrEmpty(effect.EffectKey))
                continue;

            // 동일한 EffectKey 중복 방지
            if (!usedKeys.Add(effect.EffectKey))
            {
                throw new InvalidOperationException(
                    $"Duplicate EffectKey: {effect.EffectKey}");
            }

            string key = $"{{{effect.EffectKey}}}";
            string value = FormatValue(effect);

            description = description.Replace(key, value);
        }

        return description;
    }

    private static string FormatValue(ItemEffectData effect)
    {
        float value = effect.Value;

        // 배율을 퍼센트 증가량으로 표시
        if (effect.ReferenceId == "Multiplier")
        {
            value = (value - 1f) * 100f;
        }

        return value.ToString(
            "0.##",
            CultureInfo.InvariantCulture);
    }
}