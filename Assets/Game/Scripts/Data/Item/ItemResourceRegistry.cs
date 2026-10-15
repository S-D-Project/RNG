using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Registry/Item Registry")]
public class ItemResourceRegistry : ScriptableObject
{
    [SerializeField]
    [TableList]
    private List<ItemResource> _itemRegistry;

    public ItemResource Find(string id)
    {
        foreach (ItemResource resource in _itemRegistry)
        {
            if (resource.Id == id)
            {
                return resource;
            }
        }

        return null;
    }
    
}