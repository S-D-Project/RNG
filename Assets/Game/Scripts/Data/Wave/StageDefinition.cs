using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Data/Stage Definition",
    fileName = "StageDefinition")]
public class StageDefinition : ScriptableObject
{
    [SerializeField]
    private string _id;
    
    [SerializeField]
    private List<WaveData> _waves = new List<WaveData>();

    public string Id => _id;
    public List<WaveData> Waves => _waves;
}