using System;

[Serializable]
public abstract class FireModeResourceData
{
    public abstract IFireMode Create();
}