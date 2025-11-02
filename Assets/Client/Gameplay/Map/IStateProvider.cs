namespace Client.Gameplay.Map
{
    /// Extract state snapshot for batching
    public interface IStateProvider<out TState>
    {
        TState ExtractState(uint tick);
    }
}