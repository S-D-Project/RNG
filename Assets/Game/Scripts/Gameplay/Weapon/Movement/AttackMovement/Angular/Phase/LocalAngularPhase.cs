using UnityEngine;

public class LocalAngularPhase : IAngularPhase
{
    private float _angle;
    
    public float CurrentAngle => _angle;

    public LocalAngularPhase(float startAngle)
    {
        _angle = startAngle;
    }
    
    public float Advance(float angularSpeed, float deltaTime)
    {
        _angle += angularSpeed * deltaTime;
        
        return _angle;
    }

    public void Release()
    {
    }
}