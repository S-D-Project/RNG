
    using System;
    using System.Collections.Generic;

    public class TimeIntervalHitPolicy : IHitPolicy
    {
        private readonly float _hitInterval;

        private readonly Dictionary<EnemyRuntime, float> _nextHitTimes = new();

        private float _currentTime;

        public TimeIntervalHitPolicy(float hitInterval)
        {
            if (hitInterval <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(hitInterval));
            }
            
            _hitInterval = hitInterval;
        }

        public void BeginFrame(float currentTime)
        {
            _currentTime = currentTime;
        }

        public bool TryHit(EnemyRuntime target)
        {
            if (!_nextHitTimes.TryGetValue(target, out float nextHitTime))
            {
                _nextHitTimes.Add(target, _currentTime + _hitInterval);
                return true;
            }

            if (_currentTime < nextHitTime)
            {
                return false;
            }

            _nextHitTimes[target] = _currentTime + _hitInterval;
            return true;
        }
    }
