using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpOptionView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _typeText;

    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private TextMeshProUGUI _descriptionText;

    [SerializeField]
    private Image _icon;

    public void SetOption(string typeText, string nameText, string descriptionText, Sprite icon)
    {
        _typeText.text = typeText;
        _nameText.text = name;
        _descriptionText.text = descriptionText;
        _icon.sprite = icon;
    }
}