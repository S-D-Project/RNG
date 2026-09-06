
    using UnityEngine;

    public class SpiralMovementFactory : IAttackMovementFactory
    {
        private readonly float _radius;
        private readonly float _rotationSpeed;

        public SpiralMovementFactory(float radius, float rotationSpeed)
        {
            _radius = radius;
            _rotationSpeed = rotationSpeed;
        }
        public bool TryCreateMovement(Vector2 direction, out IAttackMovement movement)
        {
            movement = new SpiralAttackMovement(_radius, _rotationSpeed);

            return true;
        }
    }
