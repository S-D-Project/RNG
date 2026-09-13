using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Registry/Enemy Resource Registry")]
public class EnemyResourceRegistry : ScriptableObject
{
    [SerializeField]
    private List<EnemyResource> _enemyResources;

    public EnemyResource Find(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        string normaliedId = id.Trim();

        foreach (EnemyResource resource in _enemyResources)
        {
            if (resource == null)
            {
                continue;
            }

            if (string.Equals(resource.Id, normaliedId, StringComparison.Ordinal))
            {
                return resource;
            }
        }
            return null;
    }

}