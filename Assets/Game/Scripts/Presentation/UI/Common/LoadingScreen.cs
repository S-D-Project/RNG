using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private UIBar _progressBar;
    
    public void Show()
    {
        gameObject.SetActive(true);
        _progressBar.SetValueImmediate(0f);

        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        
        gameObject.SetActive(false);
    }

    public void SetProgress(float progress)
    {
        _progressBar.SetValue(progress);
    }
}