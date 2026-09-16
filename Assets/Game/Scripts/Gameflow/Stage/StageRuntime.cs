
    public class StageRuntime
    {
        public StageDefinition Definition {get;}
        public int TotalKillCount { get; private set; }
        public bool IsCompleted { get; private set; }

        public StageRuntime(StageDefinition definition)
        {
            Definition = definition;
        }

        public void AddKill()
        {
            TotalKillCount++;
        }

        public void Complete()
        {
            IsCompleted = true;
        }
    }
