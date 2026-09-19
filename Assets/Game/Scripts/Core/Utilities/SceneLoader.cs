using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneLoader
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public async Awaitable<AsyncOperation> PrepareAsync(
        string sceneName,
        Action<float> onProgress = null)
    {
        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progress =
                Mathf.Clamp01(operation.progress / 0.9f);
            
            
            onProgress?.Invoke(progress);

            await Awaitable.NextFrameAsync();
        }

        onProgress?.Invoke(1f);

        return operation;
    }

    public async Awaitable ActivateAsync(AsyncOperation operation)
    {
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            await Awaitable.NextFrameAsync();
        }
    }

    public void Reload()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}