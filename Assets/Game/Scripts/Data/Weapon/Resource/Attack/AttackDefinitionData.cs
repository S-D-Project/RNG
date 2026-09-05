using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AttackDefinitionData
{

    [BoxGroup("Attack Prefab")]
    [InfoBox("Attack Prefab은 무기가 소환하는 물체")]
    [SerializeField]
    private GameObject _attackPrefab;

    [BoxGroup("Spawn Position")]
    [InfoBox("Attack이 소환되는 위치")]
    [SerializeReference]
    private SpawnPositionResourceData _spawnPosition;

    [BoxGroup("Movement")]
    [InfoBox("소환된 Attack이 움직임 정의")]
    [SerializeReference]
    private MovementResourceData _movement;

    [BoxGroup("Behaviours")]
    [InfoBox("소환된 Attack에 부여할 효과")]
    [SerializeReference]
    private List<BehaviourResourceData> _behaviours = new();

    public GameObject AttackPrefab => _attackPrefab;
    public SpawnPositionResourceData SpawnPosition => _spawnPosition;
    public MovementResourceData Movement => _movement;
    public IReadOnlyList<BehaviourResourceData> Behaviours => _behaviours;

    public void Initialize(
        GameObject attackPrefab,
        SpawnPositionResourceData spawnPosition,
        MovementResourceData movement,
        List<BehaviourResourceData> behaviours)
    {
        _attackPrefab = attackPrefab;
        _spawnPosition = spawnPosition;
        _movement = movement;
        _behaviours = behaviours;
    }
}