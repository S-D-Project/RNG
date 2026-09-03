public class WeaponRuntime
{
    public WeaponData BaseData { get; }

    public int Level { get; private set; }

    public float CurrentDamage { get; private set; }
    public float CurrentFireInterval { get; private set; }
    public float CurrentRange { get; private set; }
    
    public float CurrentSpeed { get; private set; }
    
    public float Cooldown { get; private set; }
    
    public float CurrentCastTime { get; private set; }

    public WeaponRuntime(WeaponData baseData)
    {
        BaseData = baseData;
        Level = 1;
        ReCalculateStats();
        
        Cooldown = CurrentFireInterval;
    }

    public int LevelUp()
    {
        Level++;
        ReCalculateStats();
        return Level;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (Cooldown<= 0f)
        {
            return;
        }
        
        Cooldown -= deltaTime;
    }

    public void ResetCooldown()
    {
        Cooldown = CurrentFireInterval;
    }

    private void ReCalculateStats()
    {
        CurrentDamage = BaseData.Damage + BaseData.DamagePerLevel * (Level - 1);

        CurrentFireInterval = BaseData.FireInterval + BaseData.FireIntervalPerLevel * (Level - 1);
        
        CurrentRange =  BaseData.Range + BaseData.RangePerLevel * (Level - 1);
        
        CurrentSpeed = BaseData.Speed + BaseData.SpeedPerLevel * (Level - 1);
        
        CurrentCastTime = BaseData.CastTime;
    }
        
}