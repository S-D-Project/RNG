
using UnityEngine;

/**
 * 빈 껍질 
 */
[CreateAssetMenu(menuName = "Game/Data/Player Resource")]
public class PlayerResource : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Sprite _icon;

    public string Id => _id;
    public GameObject Prefab => _prefab;
    public Sprite Icon => _icon;
}