public class WaveMovementResourceData : MovementResourceData
{
    public float Amplitude;
    public float Frequency;

    public override IAttackMovementFactory CreateFactory()
    {
        return new WaveMovementFactory(Amplitude, Frequency);
    }
}