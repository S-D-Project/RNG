using System;
using System.Collections.Generic;

public class GlobalTickHitPolicy : IHitPolicy
{
    private readonly float _tickInterval;

    private readonly HashSet<EnemyRuntime>
        _hitTargetsThisTick = new();

    private bool _isInitialized;
    private bool _isTickFrame;

    private float _nextTickTime;

    public GlobalTickHitPolicy(float tickInterval)
    {
        if (tickInterval <= 0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tickInterval));
        }

        _tickInterval = tickInterval;
    }

    public void BeginFrame(float currentTime)
    {
        _isTickFrame = false;

        if (!_isInitialized)
        {
            _isInitialized = true;

            _isTickFrame = true;

            _nextTickTime =
                currentTime + _tickInterval;

            _hitTargetsThisTick.Clear();

            return;
        }

        if (currentTime < _nextTickTime)
        {
            return;
        }

        _isTickFrame = true;

        _hitTargetsThisTick.Clear();

        do
        {
            _nextTickTime += _tickInterval;
        }
        while (_nextTickTime <= currentTime);
    }

    public bool TryHit(EnemyRuntime target)
    {
        if (!_isTickFrame)
        {
            return false;
        }

        return _hitTargetsThisTick.Add(target);
    }
}