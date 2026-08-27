using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class WeaponResourceData
{
    [SerializeField]
    private GameObject _attackPrefab;

    [SerializeReference]
    private MovementResourceData _movement;

    [SerializeReference]
    private List<BehaviourResourceData> _behaviours = new();

    public GameObject AttackPrefab => _attackPrefab;
    public MovementResourceData Movement => _movement;
    public IReadOnlyList<BehaviourResourceData> Behaviours => _behaviours;

    public void Initialize(
        GameObject attackPrefab,
        MovementResourceData movement,
        List<BehaviourResourceData> behaviours )
    {
        _attackPrefab = attackPrefab;
        _movement = movement;
        _behaviours = behaviours;
    }
}