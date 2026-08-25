using System;
using UnityEngine;
public sealed class SceneFlowManager : Singleton<SceneFlowManager>
{
    public async Awaitable EnterTile()
    {
        await SceneLoader.LoadAsync(SceneNames.TitleScene);
        
    }

    public async Awaitable EnterGame()
    {
        await SceneLoader.LoadAsync(SceneNames.GameScene);
    }
}