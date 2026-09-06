using System;

[Serializable]
public class FanFirePatternResourceData : FirePatternResourceData
{
    public int AttackCount;
    public float SpreadAngle;
    public override IFirePattern Create()
    {
        return new FanFirePattern(AttackCount,SpreadAngle);
    }
}