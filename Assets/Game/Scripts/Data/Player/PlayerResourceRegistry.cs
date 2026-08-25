using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Registry/Player Resource Registry")]
public class PlayerResourceRegistry : ScriptableObject
{
    [SerializeField]
    [TableList]
    private List<PlayerResource> _playerRegistry;

    public PlayerResource Find(string id)
    {
        foreach (PlayerResource resource in _playerRegistry)
        {
            if (resource.Id == id)
            {
                return resource;
            }
        }

        return null;
    }
}