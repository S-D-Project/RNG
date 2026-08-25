using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Title("Movement Settings")] [SerializeField] [Required]
    private InputActionReference _moveAction;

    [SerializeField] private MovementSystem _playerMovement;

    [SerializeField]
    [ReadOnly]
    private Vector2 _moveDirection;


    private void OnEnable()
    {
        _moveAction.action.Enable();
        if (_playerMovement == null)
        {
            _playerMovement = gameObject.GetComponent<MovementSystem>();
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
    }
}