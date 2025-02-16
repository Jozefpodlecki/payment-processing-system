using Microsoft.AspNetCore.SignalR.Client;
using System.Net;
using Web.Abstractions;

namespace Web.Pages
{
    public class HubConnectionWrapper : IHubConnection
    {
        private HubConnection? _hubConnection;
        private readonly string _url;

        public HubConnectionWrapper(string url)
        {
            _url = url;
        }

        public void Build()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_url)
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return _hubConnection!.StartAsync(cancellationToken);
        }

        public IDisposable On<T1>(string methodName, Action<T1> handler)
        {
            return _hubConnection!.On(methodName, handler);
        }

        public async ValueTask DisposeAsync()
        {
            await _hubConnection!.DisposeAsync();
        }
    }
}
