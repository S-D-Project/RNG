using System;

[Serializable]
public class SpreadFirePatternResourceData : FirePatternResourceData
{
    public override IFirePattern Create()
    {
        return new SpreadFirePattern();
    }
}