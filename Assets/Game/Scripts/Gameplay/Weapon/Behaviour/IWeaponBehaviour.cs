public interface IWeaponBehaviour
{
    // IWeaponBehaviour에는 규칙 / 설정 
    // 개별 Attack의 상태는 AttackRuntime의 상태로 저장 할 것.
    void OnHit(AttackRuntime attack, EnemyRuntime target);
    
}