using UnityEngine;

// Orbit 궤도 중 빈 슬롯을 얻는 책임 
public class SequentialAngularDistribution
    : IAngularDistribution
{
    private readonly SequentialAngularState _state;

    public SequentialAngularDistribution(
        int maxCount)
    {
        _state =
            new SequentialAngularState(maxCount);
    }

    public bool TryAcquire(
        Vector2 direction,
        out IAngularPhase phase)
    {
        if (!_state.TryAcquireSlot(
                out int slotIndex))
        {
            phase = null;

            return false;
        }

        phase = new SequentialAngularPhase(
            _state,
            slotIndex);

        return true;
    }

    public void SetMaxCount(
        int maxCount)
    {
        _state.SetMaxCount(maxCount);
    }
}