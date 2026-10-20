using System;
using System.Collections.Generic;

public class PlayerItemInventory
{
    private readonly List<OwnedItemEntry> _ownedItems = new();
    private readonly IReadOnlyList<OwnedItemEntry> _readOnlyItems;

    public int MaxSlots { get; }
    public int UsedSlots => _ownedItems.Count;

    public bool HasEmptySlot => UsedSlots < MaxSlots;

    public IReadOnlyList<OwnedItemEntry> OwnedItems => _readOnlyItems;

    public event Action OnInventoryChanged;

    public PlayerItemInventory(int maxSlots = 6)
    {
        if (maxSlots < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSlots));
        }

        MaxSlots = maxSlots;

        _readOnlyItems = _ownedItems.AsReadOnly();
    }


    // 특정 무기가 해당 아이템 효과를 받는지 확인
    public bool HasItem(string itemId, WeaponRuntime targetWeapon)
    {
        return TryGetItem(itemId, targetWeapon, out _);
    }

    public bool TryGetItem(string itemId, WeaponRuntime targetWeapon, out OwnedItemEntry entry)
    {
        foreach (OwnedItemEntry owned in _ownedItems)
        {
            if (owned.Item.Id != itemId)
            {
                continue;
            }

            if (!ReferenceEquals(owned.TargetWeapon, targetWeapon))
            {
                continue;
            }

            entry = owned;
            return true;
        }

        entry = null;
        return false;
    }

    // 인벤토리의 소유 / 슬롯 조건만 검사.
    public bool CanAcquire(ItemData itemData, WeaponRuntime targetWeapon)
    {
        if (itemData == null || string.IsNullOrEmpty(itemData.Id))
        {
            return false;
        }

        if (itemData.SlotType != ItemSlotType.Passive)
        {
            return false;
        }

        if (!IsValidTarget(itemData, targetWeapon))
        {
            return false;
        }

        if (HasItem(itemData.Id, targetWeapon))
        {
            return true;
        }

        return HasEmptySlot;
    }

    public ItemInventoryAcquireResult TryAcquire(ItemData itemData, WeaponRuntime targetWeapon)
    {
        if (itemData == null || string.IsNullOrEmpty(itemData.Id))
        {
            return ItemInventoryAcquireResult.InvalidItem;
        }

        if (itemData.SlotType != ItemSlotType.Passive)
        {
            return ItemInventoryAcquireResult.NotPassive;
        }

        if (!IsValidTarget(itemData, targetWeapon))
        {
            return ItemInventoryAcquireResult.InvalidTarget;
        }

        // 1. 기존 아이템 먼저 검사
        if (TryGetItem(itemData.Id, targetWeapon, out OwnedItemEntry owned))
        {
            owned.Item.LevelUp();
            OnInventoryChanged?.Invoke();

            return ItemInventoryAcquireResult.Upgraded;
        }

        // 2. 신규 아이템인 경우에만 슬롯 검사.
        if (!HasEmptySlot)
        {
            return ItemInventoryAcquireResult.Full;
        }

        // 3. 신규 아이템 생성

        ItemRuntime runtime = new ItemRuntime();
        runtime.Initialize(itemData);

        OwnedItemEntry entry = new OwnedItemEntry(runtime, targetWeapon);

        _ownedItems.Add(entry);

        OnInventoryChanged?.Invoke();

        return ItemInventoryAcquireResult.Added;
    }

    private static bool IsValidTarget(ItemData itemData, WeaponRuntime targetWeapon)
    {
        switch (itemData.TargetMode)
        {
            case ItemTargetMode.SelectWeapon:
                return targetWeapon != null;
            case ItemTargetMode.NoWeapon:
            case ItemTargetMode.AllCompatibleWeapons:
                return targetWeapon == null;
            default:
                return false;
        }
    }
}