public class WeaponRuntime
{
    public WeaponResource Resource { get; }
    public WeaponData BaseData { get; }

    public int Level { get; private set; }

    public float CurrentDamage { get; private set; }
    public float CurrentFireInterval { get; private set; }
    public float CurrentRange { get; private set; }

    public float Cooldown { get; private set; }

    public WeaponRuntime(WeaponResource resource, WeaponData baseData)
    {
        Resource = resource;
        BaseData = baseData;

        Level = 1;
        ReCalculateStats();
    }

    public int LevelUp()
    {
        Level++;
        ReCalculateStats();
        return Level;
    }

    private void ReCalculateStats()
    {
        CurrentDamage = BaseData.Damage + BaseData.DamagePerLevel * (Level - 1);

        CurrentFireInterval = BaseData.FireInterval + BaseData.FireIntervalPerLevel * (Level - 1);
        
        CurrentRange =  BaseData.Range + BaseData.RangePerLevel * (Level - 1);
    }
        
}