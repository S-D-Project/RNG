using System;

[Serializable]
public class SpreadFirePatternResourceData : FirePatternResourceData
{
    public int AttackCount;
    public float SpreadAngle = 30f;
    public override IFirePattern Create()
    {
        return new SpreadFirePattern(AttackCount,SpreadAngle);
    }
}