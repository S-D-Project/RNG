using System;
using System.Collections.Generic;
using UnityEngine;

public class SequentialAngularState
{
    private readonly List<bool> _occupiedSlots;

    private float _phase;
    private int _lastUpdatedFrame = -1;

    public int MaxCount => _occupiedSlots.Count;
    public float Phase => _phase;

    public SequentialAngularState(int maxCount)
    {
        if (maxCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxCount));
        }

        _occupiedSlots =
            new List<bool>(maxCount);

        for (int i = 0; i < maxCount; i++)
        {
            _occupiedSlots.Add(false);
        }
    }

    public bool TryAcquireSlot(
        out int slotIndex)
    {
        for (int i = 0; i < _occupiedSlots.Count; i++)
        {
            if (_occupiedSlots[i])
            {
                continue;
            }

            _occupiedSlots[i] = true;
            slotIndex = i;

            return true;
        }

        slotIndex = -1;

        return false;
    }

    public float GetSlotOffset(
        int slotIndex)
    {
        return Mathf.PI
               * 2f
               * slotIndex
               / MaxCount;
    }

    public void Advance(
        float angularSpeed,
        float deltaTime)
    {
        int currentFrame =
            Time.frameCount;

        if (_lastUpdatedFrame == currentFrame)
        {
            return;
        }

        _lastUpdatedFrame = currentFrame;

        _phase +=
            angularSpeed * deltaTime;

        _phase = Mathf.Repeat(
            _phase,
            Mathf.PI * 2f);
    }

    public void ReleaseSlot(
        int slotIndex)
    {
        _occupiedSlots[slotIndex] = false;
    }

    public void SetMaxCount(
        int maxCount)
    {
        if (maxCount < MaxCount)
        {
            throw new InvalidOperationException(
                "Orbit max count cannot be decreased.");
        }

        if (maxCount == MaxCount)
        {
            return;
        }

        int addCount =
            maxCount - MaxCount;

        for (int i = 0; i < addCount; i++)
        {
            _occupiedSlots.Add(false);
        }
    }
}