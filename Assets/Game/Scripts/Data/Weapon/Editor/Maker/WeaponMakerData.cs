using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class WeaponMakerData
{
    [LabelText("Weapon Attack Prefab")]
    public GameObject AttackPrefab;

    [LabelText("Movement")]
    public MovementType MovementType;
    
    [LabelText("Behaviours")]
    [InfoBox(
        "Add behaviours applied to this projectile.",
        InfoMessageType.None)]
    [ValidateInput(
        nameof(ValidateBehaviours),
        "Duplicate behaviours are not allowed.")]
    [ListDrawerSettings(DefaultExpandedState = true,ShowFoldout = false,DraggableItems = true)]
    public List<BehaviourType> Behaviours = new();

    private bool ValidateBehaviours(List<BehaviourType> behaviours)
    {
        if (behaviours == null)
        {
            return true;
        }
        return behaviours.Count == behaviours.Distinct().Count();
    }
}