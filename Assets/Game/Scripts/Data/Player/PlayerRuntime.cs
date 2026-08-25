/**
 * 인게임에서 사용할 Runtime 객체
 */
public class PlayerRuntime
{
    public PlayerData BaseData { get; }

    public int Level { get; private set; }
    public float Cooldown { get; private set; }


    public PlayerRuntime(PlayerData baseData)
    {
        BaseData = baseData;
        Level = 1;
    }

    private void SetCooldown(float amount)
    {
        Cooldown = amount;
    }
}