using System.Collections.Generic;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField]
    private RectTransform _popupRoot;

    [SerializeField]
    private GameObject _dimmed;

    private readonly Stack<PopupBase> _popupStack = new();

    public T Open<T>(T prefab) where T : PopupBase
    {
        if (prefab == null)
        {
            Debug.LogError("Popup open failed: Prefab is null.");
            return null;
        }

        if (_popupStack.Count > 0)
        {
            _popupStack.Peek().SetInteractable(false);
        }

        T popup = Instantiate(prefab, _popupRoot);

        // 새 Popup이 기존 Popup보다 항상 앞에 표시되도록 함
        popup.transform.SetAsLastSibling();

        _popupStack.Push(popup);

        UpdateDimmed();

        popup.Open();

        return popup;
    }

    public bool TryClose(PopupBase popup)
    {
        if (popup == null)
        {
            Debug.LogWarning("Popup close failed: Popup is null.");
            return false;
        }

        if (_popupStack.Count == 0)
        {
            Debug.LogWarning(
                $"Popup close failed: Popup stack is empty. Popup = {popup.name}");

            return false;
        }

        PopupBase topPopup = _popupStack.Peek();

        if (topPopup != popup)
        {
            Debug.LogWarning(
                $"Popup close failed: {popup.name} is not the top popup. " +
                $"Current Top = {topPopup.name}");

            return false;
        }

        CloseTopInternal();

        return true;
    }

    public PopupBase GetTop()
    {
        if (_popupStack.Count == 0)
        {
            return null;
        }

        return _popupStack.Peek();
    }

    public void CloseAll()
    {
        while (_popupStack.Count > 0)
        {
            PopupBase popup = _popupStack.Pop();

            if (popup == null)
            {
                continue;
            }

            popup.Close();
            Destroy(popup.gameObject);
        }

        UpdateDimmed();
    }

    private void CloseTopInternal()
    {
        PopupBase popup = _popupStack.Pop();

        popup.SetInteractable(false);
        popup.Close();

        Destroy(popup.gameObject);

        if (_popupStack.Count > 0)
        {
            _popupStack.Peek().SetInteractable(true);
        }

        UpdateDimmed();
    }

    private void UpdateDimmed()
    {
        _dimmed.SetActive(_popupStack.Count > 0);
    }
}