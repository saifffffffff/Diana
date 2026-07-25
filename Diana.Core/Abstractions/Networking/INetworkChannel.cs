namespace Diana.Core.Abstractions.Networking;

// networking technology 

public interface INetworkChannel : IDisposable
{
    Task SendAsync(string message);

    Task<string?> RecieveAsync();

   
}

