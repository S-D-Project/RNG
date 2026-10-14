using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [Title("References")]
    [SerializeField]
    private Button _button;

    [SerializeField]
    private TMP_Text _label;

    public event Action Clicked;

    public bool Interactable
    {
        get => _button != null && _button.interactable;
        set
        {
            if (_button != null)
            {
                _button.interactable = value;
            }
        }
    }

    private void Reset()
    {
        _button = GetComponent<Button>();

        if (_label == null)
        {
            _label = GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void Awake()
    {
        if (_button == null)
        {
            _button = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(HandleClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(HandleClick);
    }

    public void SetLabel(string text)
    {
        if (_label == null)
        {
            return;
        }

        _label.text = text;
    }

    public void SetInteractable(bool interactable)
    {
        Interactable = interactable;
    }
    
    private void HandleClick()
    {
        Clicked?.Invoke();
    }
}