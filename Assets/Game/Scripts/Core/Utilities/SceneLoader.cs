using UnityEngine.SceneManagement;
using UnityEngine;

public static class SceneLoader
{
    public static void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public static async Awaitable LoadAsync(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName);
    }

    public static void Reload()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    
}
