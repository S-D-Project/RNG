using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Weapon Definition")]
public class WeaponResource : ScriptableObject
{
    public string Id;

    public GameObject Prefab;
    public Sprite Icon;
    public AudioClip FireSound;
    public AudioClip HitSound;
}