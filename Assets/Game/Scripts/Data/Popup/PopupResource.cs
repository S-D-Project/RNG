using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Popup Resource")]
public class PopupResource : ScriptableObject
{
    [SerializeField]
    private PopupType _id;

    [SerializeField]
    private PopupBase _popupPrefab;
    
    public PopupType Id =>_id;
    public PopupBase PopupPrefab => _popupPrefab;

}