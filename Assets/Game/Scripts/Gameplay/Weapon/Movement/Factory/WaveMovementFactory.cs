using UnityEngine;

public class WaveMovementFactory : IAttackMovementFactory
{
    private readonly float _amplitude;
    private readonly float _frequency;

    public WaveMovementFactory(float amplitude, float frequency)
    {
        _amplitude = amplitude;
        _frequency = frequency;
    }
    public bool TryCreateMovement(Vector2 direction, out IAttackMovement movement)
    {
        movement = new WaveAttackMovement(_amplitude, _frequency);
        
        return true;
    }
}