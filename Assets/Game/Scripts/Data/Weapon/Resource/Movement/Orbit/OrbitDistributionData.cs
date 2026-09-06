using System;
using Sirenix.OdinInspector;

[Serializable]
public class OrbitDistributionData
{
    public AngularDistributionType Type;

    
    [ShowIf("Type",AngularDistributionType.Sequential)]
    public int MaxCount;

    public IAngularDistribution Create()
    {
        return Type switch
        {
            AngularDistributionType.Direction => new DirectionAngularDistribution(),
            AngularDistributionType.Random => new RandomAngularDistribution(),
            AngularDistributionType.Sequential => new SequentialAngularDistribution(MaxCount),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}