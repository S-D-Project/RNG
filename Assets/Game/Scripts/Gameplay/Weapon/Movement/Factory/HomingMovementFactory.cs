
    using UnityEngine;

    public class HomingMovementFactory : IAttackMovementFactory
    {
        private readonly float _turnSpeed;
        private readonly float _searchInterval;
        
        public HomingMovementFactory(float turnSpeed, float searchInterval)
        {
            _turnSpeed = turnSpeed;
            _searchInterval = searchInterval;
        }
        
        public bool TryCreateMovement(Vector2 direction, out IAttackMovement movement)
        {
            movement = new HomingAttackMovement(_turnSpeed, _searchInterval);

            return true;
        }
    }
