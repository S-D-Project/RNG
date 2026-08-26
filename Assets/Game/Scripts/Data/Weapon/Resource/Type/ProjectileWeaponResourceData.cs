using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileWeaponResourceData : WeaponTypeResourceData
{
    [SerializeField]
    private GameObject _projectilePrefab;

    [SerializeReference]
    private ProjectileMovementResourceData _movement;

    [SerializeReference]
    private List<ProjectileBehaviourResourceData> _behaviours = new();

    public GameObject ProjectilePrefab => _projectilePrefab;
    public ProjectileMovementResourceData Movement => _movement;
    public IReadOnlyList<ProjectileBehaviourResourceData> Behaviours => _behaviours;

    public void Initialize(
        GameObject projectilePrefab,
        ProjectileMovementResourceData movement,
        List<ProjectileBehaviourResourceData> behaviours )
    {
        _projectilePrefab = projectilePrefab;
        _movement = movement;
        _behaviours = behaviours;
    }
}