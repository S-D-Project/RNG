using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class LevelUpPopup : PopupBase
{
    [SerializeField]
    private TextMeshProUGUI _titleText;   
    
    [SerializeField]
    private TextMeshProUGUI _subTitleText;

    [SerializeField]
    private RectTransform _optionArea;

    [SerializeField]
    private LevelUpOptionView[] _optionViews;

    private Action<LevelUpOption> _onSelected;
    
    protected override void OnOpen()
    {
        
        
    }
    
    
}