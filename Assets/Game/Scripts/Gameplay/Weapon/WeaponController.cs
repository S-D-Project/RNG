using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private WeaponRuntime _weaponRuntime;
    private ITargeting _targeting;
    private IFirePattern _firePattern;

    private float _remainingCooldown;
    private bool _isInitialized;
    private AttackRuntimeManager _attackRuntimeManager;

    public AttackRuntimeManager AttackRuntimeManager => _attackRuntimeManager;

    public void Initialize(WeaponRuntime runtime,AttackRuntimeManager attackRuntimeManager)
    {
        if (_isInitialized)
        {
            return;
        }
        
        _weaponRuntime = runtime;
        _targeting = runtime.BaseData.Targeting.Create();
        _firePattern = runtime.BaseData.FirePattern.Create();
        _attackRuntimeManager = attackRuntimeManager;

        _remainingCooldown = 0f;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }
        UpdateCooldown();
        
        
        if (_remainingCooldown > 0f)
        {
            return;
        }
        
        TryFire();
        
    }

    private void UpdateCooldown()
    {
        if (_remainingCooldown <= 0f)
        {
            return;
        }
        _remainingCooldown -= Time.deltaTime;
    }

    private void TryFire()
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        EnemyRuntime target = _targeting.FindTarget(position, EnemyManager.Instance.EnemyList);

        if (target == null)
        {
            return;
        }
        
        _firePattern.Fire(this,_weaponRuntime,target);

        _remainingCooldown = _weaponRuntime.CurrentFireInterval;
    }
}