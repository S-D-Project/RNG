using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneFlowManager : Singleton<SceneFlowManager>
{
    [SerializeField]
    private LoadingScreen _loadingScreen;

    [SerializeField]
    [Required]
    private SceneLoader _sceneLoader;

    private bool _isLoading;
    private float _completeDelay = 1f;

    public async Awaitable LoadSceneAsync(string sceneName)
    {
        if (_isLoading)
        {
            return;
        }

        _isLoading = true;

        try
        {
            _loadingScreen.Show();

            await WaitRealTime(_completeDelay);
            
            AsyncOperation operation =
                await _sceneLoader.PrepareAsync(
                    sceneName,
                    _loadingScreen.SetProgress);
            
            await WaitRealTime(_completeDelay);
            
            await _sceneLoader.ActivateAsync(operation);
            
            _loadingScreen.Hide();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Awaitable WaitRealTime(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            await Awaitable.NextFrameAsync();
        }
    }
}