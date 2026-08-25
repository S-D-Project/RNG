using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public PlayerRuntime SpawnPlayer(Vector3 position,string characterId)
    {
        PlayerData playerData = GameDataStore.Instance.GetPlayerData(characterId);
        
        GameObject playerObject = Instantiate(playerData.Prefab,position,Quaternion.identity);
        
        PlayerRuntime runtime = playerObject.GetComponent<PlayerRuntime>();
        runtime.Initialize(playerData);

        return runtime;
    }
}