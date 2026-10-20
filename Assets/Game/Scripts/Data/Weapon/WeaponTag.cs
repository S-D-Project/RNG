using System;

[Flags]
public enum WeaponTag
{
    None = 0,
    
    Projectile = 1<<0,
    Area = 1 << 1 ,
    Magic = 1 <<2 ,
    Special = 1 <<3 
}