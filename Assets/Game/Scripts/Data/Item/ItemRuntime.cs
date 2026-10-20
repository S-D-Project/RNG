public class ItemRuntime
{
    public ItemData BaseData { get; private set; }

    public string Id { get; private set; }
    public int Level { get; private set; }

    private bool _isInitialized = false;

    public void Initialize(ItemData baseData)
    {
        if (_isInitialized)
        {
            return;
        }

        BaseData = baseData;
        Id = baseData.Id;
        Level = 1;
        _isInitialized = true;
    }

    public int LevelUp()
    {
        Level++;
        return Level;
    }
}