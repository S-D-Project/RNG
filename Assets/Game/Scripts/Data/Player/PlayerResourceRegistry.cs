
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game/Data/Registry/Player Resource Registry")]
public class PlayerResourceRegistry :ScriptableObject
{
    [SerializeField] private List<PlayerResource> _playerRegistry;
    
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