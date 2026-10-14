using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private float _lerpSpeed = 5f;

    private float _targetValue;

    public float Value => _slider != null
        ? _slider.value
        : 0f;

    private void Reset()
    {
        _slider = GetComponent<Slider>();
    }

    private void Awake()
    {
        if (_slider == null)
        {
            _slider = GetComponent<Slider>();
        }

        _targetValue = _slider.value;
    }

    private void Update()
    {
        if (_slider == null)
        {
            return;
        }

        _slider.value = Mathf.Lerp(
            _slider.value,
            _targetValue,
            _lerpSpeed * Time.unscaledDeltaTime);

        if (Mathf.Abs(_slider.value - _targetValue) < 0.001f)
        {
            _slider.value = _targetValue;
        }
    }

    public void SetValue(float value)
    {
        _targetValue = Mathf.Clamp01(value);
    }

    public void SetValueImmediate(float value)
    {
        float clampedValue = Mathf.Clamp01(value);

        _targetValue = clampedValue;
        _slider.value = clampedValue;
    }
}