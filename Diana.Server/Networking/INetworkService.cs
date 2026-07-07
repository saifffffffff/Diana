namespace Diana.Server.Networking;

// networking technology 

interface INetworkService : IDisposable
{

    Task SendAsync(string message );


    Task<string?> RecieveAsync();


}
