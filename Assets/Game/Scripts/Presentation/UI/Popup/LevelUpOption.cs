using System;
using UnityEngine;

public class LevelUpOption
{
    public Sprite Icon { get; }
    public string TypeText { get; }
    public string Name { get; }
    public string Description { get; }
    public string LevelText { get; }

    private readonly Action _onSelect;

    public LevelUpOption(
        Sprite icon,
        string typeText,
        string name,
        string description,
        string levelText,
        Action onSelect)
    {
        Icon = icon;
        TypeText = typeText;
        Name = name;
        Description = description;
        LevelText = levelText;

        _onSelect = onSelect;
    }

    public void Select()
    {
        _onSelect?.Invoke();
    }
}