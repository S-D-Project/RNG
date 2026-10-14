using UnityEngine;
using UnityEngine.UI;

public class UIIconView : MonoBehaviour
{
    [SerializeField]
    private Image _iconImage;

    private Sprite Icon => _iconImage != null ? _iconImage.sprite : null;

    private void Reset()
    {
        if(_iconImage == null)
        {
            _iconImage = GetComponent<Image>();
        }
    }

    public void SetIcon(Sprite icon)
    {
        if (_iconImage == null)
        {
            return;
        }

        _iconImage.sprite = icon;
        _iconImage.enabled = icon != null;
    }

    public void Clear()
    {
        SetIcon(null);
    }
}