namespace Web.Abstractions
{
    public interface IHubConnection : IAsyncDisposable
    {
        void Build();
        Task StartAsync(CancellationToken cancellationToken = default);
        IDisposable On<T1>(string methodName, Action<T1> handler);
    }
}
