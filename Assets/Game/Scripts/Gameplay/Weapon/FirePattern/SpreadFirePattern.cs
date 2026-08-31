
using UnityEngine;

public class SpreadFirePattern : IFirePattern
{
    private readonly int _attackCount;
    private readonly float _spreadAngle;
    
    public SpreadFirePattern(int attackCount, float spreadAngle)
    {
        _attackCount = attackCount;
        _spreadAngle = spreadAngle;
    }
    
    public void Fire(WeaponController controller, WeaponRuntime runtime, EnemyRuntime target)
    {
        if (target == null)
        {
            return;
        }

        Vector2 origin = controller.transform.position;
        Vector2 targetPosition = target.transform.position;
        
        Vector2 baseDirection = (targetPosition - origin).normalized;

        if (_attackCount <= 1)
        {
            controller.FireAttack(baseDirection);
            return;
        }

        float startAngle = -_spreadAngle * 0.5f;
        float angleStep = _spreadAngle / (_attackCount - 1);


        for (int i = 0; i < _attackCount; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector2 direction = Rotate(baseDirection, angle);
            controller.FireAttack(direction);
        }
        

    }
    
    private Vector2 Rotate(Vector2 direction,float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(direction.x * cos - direction.y * sin, direction.x * sin + direction.y * cos);
    }
}