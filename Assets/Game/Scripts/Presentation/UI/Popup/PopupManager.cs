using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [Title("Popup Settings")]

    [SerializeField]
    private PopupRegistry _popupRegistry;

    [SerializeField]
    private RectTransform _popupRoot;

    [SerializeField]
    private GameObject _dimmed;

    private readonly Stack<PopupBase> _popupStack = new();

    private Dictionary<PopupType, PopupBase> _popupPrefabs;

    public override void OnInitialize()
    {
        _popupPrefabs = new Dictionary<PopupType, PopupBase>();

        List<PopupResource> popupResources =
            _popupRegistry.GetAllPopupResource();

        foreach (PopupResource resource in popupResources)
        {
            if (resource == null || resource.PopupPrefab == null)
            {
                continue;
            }

            if (_popupPrefabs.ContainsKey(resource.Id))
            {
                Debug.LogError(
                    $"Duplicate Popup Id: {resource.Id}");

                continue;
            }

            _popupPrefabs.Add(
                resource.Id,
                resource.PopupPrefab);
        }

        UpdateDimmed();
    }

    public T Open<T>(PopupType popupId) where T : PopupBase
    {
        if (!_popupPrefabs.TryGetValue(
                popupId,
                out PopupBase prefab))
        {
            Debug.LogError(
                $"Popup not found. Id: {popupId}");

            return null;
        }

        if (prefab is not T typedPrefab)
        {
            Debug.LogError(
                $"Popup type mismatch. " +
                $"Id: {popupId}, " +
                $"Expected: {typeof(T).Name}, " +
                $"Actual: {prefab.GetType().Name}");

            return null;
        }

        return Open(typedPrefab);
    }

    private T Open<T>(T prefab) where T : PopupBase
    {
        if (prefab == null)
        {
            Debug.LogError(
                "Popup open failed: Prefab is null.");

            return null;
        }

        if (_popupStack.Count > 0)
        {
            _popupStack
                .Peek()
                .SetInteractable(false);
        }

        T popup = Instantiate(
            prefab,
            _popupRoot);

        popup.transform.SetAsLastSibling();

        _popupStack.Push(popup);

        popup.Open();

        UpdateDimmed();

        return popup;
    }

    public bool TryClose(PopupBase popup)
    {
        if (popup == null)
        {
            Debug.LogWarning(
                "Popup close failed: Popup is null.");

            return false;
        }

        if (_popupStack.Count == 0)
        {
            Debug.LogWarning(
                $"Popup close failed: Stack is empty. " +
                $"Popup = {popup.name}");

            return false;
        }

        PopupBase topPopup =
            _popupStack.Peek();

        if (topPopup != popup)
        {
            Debug.LogWarning(
                $"Popup close failed: " +
                $"{popup.name} is not Top. " +
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
            PopupBase popup =
                _popupStack.Pop();

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
        PopupBase popup =
            _popupStack.Pop();

        popup.SetInteractable(false);
        popup.Close();

        Destroy(popup.gameObject);

        if (_popupStack.Count > 0)
        {
            _popupStack
                .Peek()
                .SetInteractable(true);
        }

        UpdateDimmed();
    }

    private void UpdateDimmed()
    {
        _dimmed.SetActive(
            _popupStack.Count > 0);
    }
}