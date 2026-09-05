public interface IMovement
{
    void Move(AttackRuntime attack, float deltaTime);
    void Initialize(AttackRuntime attack);
}