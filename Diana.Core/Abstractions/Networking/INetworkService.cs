namespace Diana.Core.Abstractions.Networking;

// networking technology 

public interface INetworkService : IDisposable
{
    Task SendAsync(string message);

    Task<string?> RecieveAsync();
}

