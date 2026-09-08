public class SequentialAngularPhase
    : IAngularPhase
{
    private readonly SequentialAngularState _state;
    private readonly int _slotIndex;

    private bool _isReleased;

    public float CurrentAngle =>
        _state.Phase
        + _state.GetSlotOffset(_slotIndex);

    public SequentialAngularPhase(
        SequentialAngularState state,
        int slotIndex)
    {
        _state = state;
        _slotIndex = slotIndex;
    }

    public float Advance(
        float angularSpeed,
        float deltaTime)
    {
        _state.Advance(
            angularSpeed,
            deltaTime);

        return CurrentAngle;
    }

    public void Release()
    {
        if (_isReleased)
        {
            return;
        }

        _isReleased = true;

        _state.ReleaseSlot(_slotIndex);
    }
}