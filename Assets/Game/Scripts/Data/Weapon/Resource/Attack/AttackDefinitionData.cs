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

    [BoxGroup("Hit Policy")]
    [InfoBox("동일 Enemy에 대한 Attack의 Hit 가능 조건")]
    [SerializeReference]
    private HitPolicyResourceData _hitPolicy;

    [BoxGroup("Behaviours")]
    [InfoBox("소환된 Attack에 부여할 효과")]
    [SerializeReference]
    private List<BehaviourResourceData> _behaviours = new();

    public GameObject AttackPrefab => _attackPrefab;
    public SpawnPositionResourceData SpawnPosition => _spawnPosition;
    public MovementResourceData Movement => _movement;
    public HitPolicyResourceData HitPolicy => _hitPolicy;
    public IReadOnlyList<BehaviourResourceData> Behaviours => _behaviours;
    
    public void Initialize(
        GameObject attackPrefab,
        SpawnPositionResourceData spawnPosition,
        MovementResourceData movement,
        HitPolicyResourceData hitPolicy,
        List<BehaviourResourceData> behaviours)
    {
        _attackPrefab = attackPrefab;
        _spawnPosition = spawnPosition;
        _movement = movement;
        _hitPolicy = hitPolicy;
        _behaviours = behaviours;
    }

    public bool Validate(
        TargetingResourceData targeting,
        out string error)
    {
        if (_attackPrefab == null)
        {
            error = "Attack Prefab is null.";
            return false;
        }

        if (_spawnPosition == null)
        {
            error = "Spawn Position is null.";
            return false;
        }

        if (_movement == null)
        {
            error = "Movement is null.";
            return false;
        }

        if (_hitPolicy == null)
        {
            error = "Hit Policy is null.";
            return false;
        }

        if (_behaviours == null)
        {
            error = "Behaviours is null.";
            return false;
        }

        if (targeting == null)
        {
            error = "Targeting is null.";
            return false;
        }

        bool requiresTarget = targeting.RequiresTarget;

        if (!requiresTarget)
        {
            if (_spawnPosition is TargetSpawnPositionResourceData)
            {
                error =
                    "Target Spawn Position requires a Targeting that provides a target.";

                return false;
            }

            if (_spawnPosition is BetweenSpawnPositionResourceData)
            {
                error =
                    "Between Spawn Position requires a Targeting that provides a target.";

                return false;
            }

            if (_movement is FollowMovementResourceData followMovement &&
                followMovement.TargetType == FollowTargetType.Target)
            {
                error =
                    "Follow Target movement requires a Targeting that provides a target.";

                return false;
            }
        }

        error = null;
        return true;
    }
}