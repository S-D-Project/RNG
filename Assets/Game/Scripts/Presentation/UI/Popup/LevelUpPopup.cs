using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelUpPopup : PopupBase
{
    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private TextMeshProUGUI _subTitleText;

    [SerializeField]
    private LevelUpOptionView[] _optionViews;

    private Action _onCompleted;

    public void SetData(
        int playerLevel,
        IReadOnlyList<LevelUpOption> options,
        Action onCompleted = null)
    {
        _titleText.text = "LEVEL UP!";
        _subTitleText.text = $"LV. {playerLevel}";

        _onCompleted = onCompleted;

        for (int i = 0; i < _optionViews.Length; i++)
        {
            LevelUpOptionView view = _optionViews[i];

            if (i >= options.Count)
            {
                view.gameObject.SetActive(false);
                continue;
            }

            view.gameObject.SetActive(true);
            view.SetData(options[i], OnOptionSelected);
        }
    }

    private void OnOptionSelected(LevelUpOption option)
    {
        if (option == null)
        {
            return;
        }

        // TryClose → OnClose가 호출되면서
        // _onCompleted가 초기화될 수 있으므로 먼저 보관
        Action completedCallback = _onCompleted;

        if (!PopupManager.Instance.TryClose(this))
        {
            return;
        }

        option.Select();

        completedCallback?.Invoke();
    }

    protected override void OnClose()
    {
        base.OnClose();

        _onCompleted = null;
    }
}