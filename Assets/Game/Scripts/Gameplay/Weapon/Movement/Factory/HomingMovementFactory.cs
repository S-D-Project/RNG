
    using UnityEngine;

    public class HomingMovementFactory : IAttackMovementFactory
    {
        private readonly float _turnSpeed;
        private readonly float _searchInterval;
        private readonly float _searchRadius;
        
        public HomingMovementFactory(float turnSpeed, float searchInterval,float searchRadius)
        {
            _turnSpeed = turnSpeed;
            _searchInterval = searchInterval;
            _searchRadius = searchRadius;
        }
        
        public bool TryCreateMovement(Vector2 direction, out IAttackMovement movement)
        {
            movement = new HomingAttackMovement(_turnSpeed, _searchInterval, _searchRadius);

            return true;
        }
    }
