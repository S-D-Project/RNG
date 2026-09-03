using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponControllerManager : MonoBehaviour
{
    private readonly List<WeaponController> _weaponControllers = new();
    
    public IReadOnlyList<WeaponController> WeaponControllers => _weaponControllers;

    public void AddWeapon(WeaponRuntime runtime, WeaponController controller)
    {
        if (runtime == null || controller == null)
        {
            return;
        }
        
        _weaponControllers.Add(controller);
    }

    public void SetOwnerMoving(bool isMoving)
    {
        foreach (WeaponController controller in _weaponControllers)
        {
            controller.SetOwnerMoving(isMoving);
        }
    }

}