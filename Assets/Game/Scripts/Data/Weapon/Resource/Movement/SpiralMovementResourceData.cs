
    public class SpiralMovementResourceData : MovementResourceData
    {
        public float Radius = 1f;
        public float RotationSpeed = 360f;
        public override IAttackMovementFactory CreateFactory()
        {
            return new SpiralMovementFactory(Radius, RotationSpeed);
        }
    }
