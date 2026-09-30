using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PopupBase : MonoBehaviour
{
    public bool IsOpened { get; private set; }

    private CanvasGroup _canvasGroup;

    private CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            return _canvasGroup;
        }
    }

    public virtual void Open()
    {
        if (IsOpened)
        {
            return;
        }

        gameObject.SetActive(true);
        IsOpened = true;

        SetInteractable(true);

        OnOpen();
    }

    public virtual void Close()
    {
        if (!IsOpened)
        {
            return;
        }

        OnClose();

        IsOpened = false;
        gameObject.SetActive(false);
    }

    public void SetInteractable(bool interactable)
    {
        CanvasGroup.interactable = interactable;
        CanvasGroup.blocksRaycasts = interactable;
    }

    protected virtual void OnOpen()
    {
    }

    protected virtual void OnClose()
    {
    }
}