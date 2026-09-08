public interface IAttackMovement
{
    void Initialize(AttackRuntime attack);
    void Move(AttackRuntime attack, float deltaTime);
    void OnRelease(AttackRuntime attack);
}