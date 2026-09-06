public interface IAttackMovement
{
    void Move(AttackRuntime attack, float deltaTime);
    void Initialize(AttackRuntime attack);

    void OnRelease(AttackRuntime attack);
}