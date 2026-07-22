using Diana.Core.Dtos;

namespace Diana.Core.Abstractions.Services;


public interface IChatService
{


    void Connect(string ipAddress , int port );
    
    void Disconnect();

    Task SendAsync(ClientRequest msg);

    protected void Recieve();

    public bool IsConnected { get;  }

    public event Action <ServerResponse> OnMessageReceived;


}
