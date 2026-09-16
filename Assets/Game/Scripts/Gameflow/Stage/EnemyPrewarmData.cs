public class EnemyPrewarmData
{
    public EnemyData EnemyData { get; }
    public int Count;

    public EnemyPrewarmData(EnemyData enemyData, int count)
    {
        EnemyData = enemyData;
        Count = count;
    }
}