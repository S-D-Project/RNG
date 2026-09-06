public interface IAngularPhase
{
    float CurrentAngle { get; }
    float Advance(float angularSpeed, float deltaTime);

    void Release();
}