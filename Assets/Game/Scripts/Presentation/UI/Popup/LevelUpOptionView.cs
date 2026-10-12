using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpOptionView : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    [SerializeField]
    private TextMeshProUGUI _typeText;

    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private TextMeshProUGUI _descriptionText;

    [SerializeField]
    private Image _icon;

    private LevelUpOption _option;
    private Action<LevelUpOption> _onSelected;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    public void SetData(
        LevelUpOption option,
        Action<LevelUpOption> onSelected)
    {
        _option = option;
        _onSelected = onSelected;

        _icon.sprite = option.Icon;
        _typeText.text = option.TypeText;
        _nameText.text = option.Name;
        _descriptionText.text = option.Description;

        bool hasLevel =
            !string.IsNullOrEmpty(option.LevelText);

        _levelText.gameObject.SetActive(hasLevel);

        if (hasLevel)
        {
            _levelText.text = option.LevelText;
        }
    }

    private void OnClick()
    {
        if (_option == null)
        {
            return;
        }

        _onSelected?.Invoke(_option);
    }
}