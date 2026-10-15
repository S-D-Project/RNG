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

        if (_weaponControllers.Count > 3)
        {

            return;
        }
        Debug.Log($"무기 추가 : {runtime.BaseData.Id}");
        _weaponControllers.Add(controller);
        Debug.Log($"현재 무기 수 : {WeaponControllers.Count}");
    }

    public void SetOwnerValue(PlayerWeaponDto dto)
    {
        foreach (WeaponController controller in _weaponControllers)
        {
            controller.SetWeaponControllerValueFromPlayerInput(dto);
        }
    }

}