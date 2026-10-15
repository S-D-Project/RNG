public class ItemEffectData
{
    public string EffectKey { get; }
    public ItemEffectType EffectType { get; }
    public string ReferenceId { get; }
    public float Value { get; }

    public ItemEffectData(ItemEffectDto dto)
    {
        EffectKey = dto.EffectKey;
        EffectType = dto.EffectType;
        ReferenceId = dto.ReferenceId;
        Value = dto.Value;
    }
}