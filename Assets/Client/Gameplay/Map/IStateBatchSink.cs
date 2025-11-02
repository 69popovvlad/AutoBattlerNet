namespace Client.Gameplay.Map
{
    /// Where to send batch
    public interface IStateBatchSink<in TState>
    {
        void SendBatch(uint tick, TState[] items);
    }
}