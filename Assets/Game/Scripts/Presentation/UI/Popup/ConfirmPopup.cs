using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopup : PopupBase
{
    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private TextMeshProUGUI _messageText;

    [SerializeField]
    private TextMeshProUGUI _okText;

    [SerializeField]
    private TextMeshProUGUI _cancelText;

    [SerializeField]
    private Button _okButton;

    [SerializeField]
    private Button _cancelButton;

    private Action _onConfirm;
    private Action _onCancel;

    private Func<Awaitable> _onConfirmAsync;

    private void Awake()
    {
        _okButton.onClick.AddListener(OnClickOk);
        _cancelButton.onClick.AddListener(OnClickCancel);
    }

    public void SetData(
        string title,
        string message,
        string okText,
        string cancelText,
        Action onConfirm = null,
        Action onCancel = null)
    {
        SetText(
            title,
            message,
            okText,
            cancelText);

        _onConfirm = onConfirm;
        _onConfirmAsync = null;

        _onCancel = onCancel;
    }

    public void SetAsyncData(
        string title,
        string message,
        string okText,
        string cancelText,
        Func<Awaitable> onConfirm,
        Action onCancel = null)
    {
        SetText(
            title,
            message,
            okText,
            cancelText);

        _onConfirm = null;
        _onConfirmAsync = onConfirm;

        _onCancel = onCancel;
    }

    private async void OnClickOk()
    {
        Action callback = _onConfirm;
        Func<Awaitable> asyncCallback = _onConfirmAsync;

        // 현재 Popup이 정상적으로 닫힌 경우에만
        // Confirm Callback을 실행한다.
        if (!PopupManager.Instance.TryClose(this))
        {
            return;
        }

        if (asyncCallback != null)
        {
            await asyncCallback.Invoke();
            return;
        }

        callback?.Invoke();
    }

    private void OnClickCancel()
    {
        Action callback = _onCancel;

        if (!PopupManager.Instance.TryClose(this))
        {
            return;
        }

        callback?.Invoke();
    }

    protected override void OnClose()
    {
        base.OnClose();

        _onConfirm = null;
        _onConfirmAsync = null;
        _onCancel = null;
    }

    private void SetText(
        string title,
        string message,
        string okText,
        string cancelText)
    {
        _titleText.text = title;
        _messageText.text = message;
        _okText.text = okText;
        _cancelText.text = cancelText;
    }
}