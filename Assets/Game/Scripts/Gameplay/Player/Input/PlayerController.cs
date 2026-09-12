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


    private Vector2 _moveDirection;
    private Vector2 _lastDirection = Vector3.right;

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

        UpdateWeaponState();
    }

    private void UpdateWeaponState()
    {
        bool isMoving = _moveDirection.sqrMagnitude > 0.001f;
        if (isMoving)
        {
            _lastDirection = _moveDirection;
        }

        _weaponControllerManager.SetOwnerValue(new PlayerWeaponDto(isMoving, _lastDirection));
    }
}

public struct PlayerWeaponDto
{
    public bool IsMoving;
    public Vector2 MoveDirection;


    public PlayerWeaponDto(bool isMoving, Vector2 moveDirection)
    {
        IsMoving = isMoving;
        MoveDirection = moveDirection;
    }
}