using System;

[Serializable]
public class SingleFirePatternResourceData : FirePatternResourceData
{
    public override IFirePattern Create()
    {
        return new SingleFirePattern();
    }
}