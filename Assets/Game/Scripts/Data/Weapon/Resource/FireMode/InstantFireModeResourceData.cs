using System;

[Serializable]
public class InstantFireModeResourceData : FireModeResourceData
{
    public override IFireMode Create()
    {
        return new InstantFireMode();
    }
}