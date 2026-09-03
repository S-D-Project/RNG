using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Title("Movement Settings")]
    [SerializeField]
    [Required]
    private InputActionReference _moveAction;

    [SerializeField]
    private MovementSystem _playerMovement;

    [SerializeField]
    [ReadOnly]
    private Vector2 _moveDirection;

    [SerializeField]
    private PlayerWeaponControllerManager _weaponControllerManager;

    private List<WeaponController> _weaponControllers;
    
    private void OnEnable()
    {
        _moveAction.action.Enable();
        if (_playerMovement == null)
        {
            _playerMovement = gameObject.GetComponent<MovementSystem>();
        }

        if (_weaponControllerManager == null)
        {
            _weaponControllerManager = GetComponent<PlayerWeaponControllerManager>();
        }
    }

    private void OnDisable()
    {
        _moveAction.action.Disable();
    }

    private void Update()
    {
        _moveDirection = _moveAction.action.ReadValue<Vector2>();
        _playerMovement.SetMoveDirection(_moveDirection);

        UpdateWeaponMovementState();
    }

    private void UpdateWeaponMovementState()
    {
        bool isMoving = _moveDirection.sqrMagnitude > 0.001f;

        _weaponControllerManager.SetOwnerMoving(isMoving);
    }
}