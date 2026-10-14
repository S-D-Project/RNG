using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Registry/Popup Registry")]
public class PopupRegistry : ScriptableObject
{
    [SerializeField]
    [TableList]
    private List<PopupResource> _popupList;
    
    public List<PopupResource> GetAllPopupResource()
    {
        return _popupList;
    }
}