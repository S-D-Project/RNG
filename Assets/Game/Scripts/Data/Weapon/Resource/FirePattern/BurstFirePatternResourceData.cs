using System;

[Serializable]
public class BurstFirePatternResourceData : FirePatternResourceData
{
    public override IFirePattern Create()
    {
        return new BurstFirePattern();
    }
}