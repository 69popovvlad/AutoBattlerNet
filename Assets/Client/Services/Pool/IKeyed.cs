namespace Client.Services.Pool
{
    public interface IKeyed<TKey>
    {
        TKey Key { get; }
    }
}