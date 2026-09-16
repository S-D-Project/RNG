using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Enemy Resource")]
public class EnemyResource : ScriptableObject
{
    [SerializeField]
    private string _id;

    [SerializeField]
    private EnemyRuntime _prefab;

    public string Id => _id;
    public EnemyRuntime Prefab =>  _prefab;
}