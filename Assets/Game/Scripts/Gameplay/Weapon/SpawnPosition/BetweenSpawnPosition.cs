using UnityEngine;

public class BetweenSpawnPosition : SpawnPosition
{
    // 0~1 -> 0이면 Owner, 1이면 Target에 가깝게
    private readonly float _ratio;
    
    public BetweenSpawnPosition(float ratio, Vector2 offset) : base(offset)
    {
        _ratio = Mathf.Clamp01(ratio);
        
    }

    protected override Vector2 GetBasePosition(Vector2 ownerPosition, Vector2 targetPosition)
    {
        return Vector2.Lerp(ownerPosition,targetPosition, _ratio);
    }
}