using System;

[Serializable]
public class CastingFireModeResourceData : FireModeResourceData
{
    public override IFireMode Create()
    {
        return new CastingFireMode();
    }
}