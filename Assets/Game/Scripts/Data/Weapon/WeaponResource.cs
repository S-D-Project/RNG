using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Weapon Resource")]
public class WeaponResource : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Sprite _icon;
    [SerializeField] private AudioClip _fireSound;
    [SerializeField] private AudioClip _hitSound;

    public string Id => _id;

    public GameObject Prefab => _prefab;
    public Sprite Icon => _icon;
    public AudioClip FireSound => _fireSound;
    public AudioClip HitSound => _hitSound;
}