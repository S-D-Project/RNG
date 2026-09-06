using UnityEngine;

// 지금 orbit에 attack 하나를 추가할 수 있는지 판단하고, 가능하면 시작 각도를 배치 
public interface IAngularDistribution
{
    bool TryAcquire(Vector2 direction, out IAngularPhase phase);
}