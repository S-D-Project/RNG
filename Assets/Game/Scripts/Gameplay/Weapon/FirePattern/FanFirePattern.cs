
using System.Collections.Generic;
using UnityEngine;

public class FanFirePattern : IFirePattern
{
    private readonly int _attackCount;
    private readonly float _spreadAngle;
    
    public FanFirePattern(int attackCount, float spreadAngle)
    {
        _attackCount = attackCount;
        _spreadAngle = spreadAngle;
    }

    public IReadOnlyList<Vector2> GetDirections(Vector2 baseDirection)
    {
        List<Vector2> directions = new();

        if (_attackCount <= 1)
        {
            directions.Add(baseDirection);
            return directions;
        }
        
        float startAngle = -_spreadAngle * 0.5f;
        float angleStep = _spreadAngle / (_attackCount - 1);

        for (int i = 0; i < _attackCount; i++)
        {
            float angle = startAngle + angleStep * i;

            directions.Add(Rotate(baseDirection, angle));
            
        }

        return directions;
    }
    
    private Vector2 Rotate(Vector2 direction,float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(direction.x * cos - direction.y * sin, direction.x * sin + direction.y * cos);
    }
}