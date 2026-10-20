using System.Collections.Generic;
using System.Linq;
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

        if (_weaponControllers.Count > 3)
        {

            return;
        }
        _weaponControllers.Add(controller);

    }

    public IReadOnlyList<WeaponRuntime> GetWeaponRuntimes()
    {
        return WeaponControllers.Select(x => x.GetWeaponRuntime()).ToList();
    }

    public void SetOwnerValue(PlayerWeaponDto dto)
    {
        foreach (WeaponController controller in _weaponControllers)
        {
            controller.SetWeaponControllerValueFromPlayerInput(dto);
        }
    }

}