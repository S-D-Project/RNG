using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class ProjectileWeaponMakerData
{
    [LabelText("Projectile Prefab")]
    public GameObject ProjectilePrefab;

    [LabelText("Movement")]
    public ProjectileMovementType MovementType;
    
    [LabelText("Behaviours")]
    [InfoBox(
        "Add behaviours applied to this projectile.",
        InfoMessageType.None)]
    [ValidateInput(
        nameof(ValidateBehaviours),
        "Duplicate behaviours are not allowed.")]
    [ListDrawerSettings(DefaultExpandedState = true,ShowFoldout = false,DraggableItems = true)]
    public List<ProjectileBehaviourType> Behaviours = new();

    private bool ValidateBehaviours(List<ProjectileBehaviourType> behaviours)
    {
        if (behaviours == null)
        {
            return true;
        }
        return behaviours.Count == behaviours.Distinct().Count();
    }
}