using System.Net.WebSockets;
using System.Text.Json;
using Diana.Core.Dtos;
using Diana.Core;
using System.Net;
using Diana.Server.Messaging;
using Diana.Server.Networking;
using Diana.Server.Rooms;
using Diana.Server.Serialization;
namespace Diana.Server.Sessions;

// TODO : Solve encapsulation issue : private setters + setters methods
// managing the lifecycle of one connected client's session
class ClientSessionHandler : IAsyncDisposable
{
    public string? Username { get;  set; }
    public string? ImageBase64 { get; set; }
    public ChatRoom? CurrentRoom { get;  set; }
    public IPEndPoint? EndPoint { get;  set; }
    public bool IsMuted { get; set; }
    public bool IsSpeaking { get; set; }
    bool _disposed;

    INetworkService _networkService;
    ISerializer _serializer;

    public ClientSessionHandler(INetworkService networkService , ISerializer serializer)
    {
        _networkService = networkService;
        _serializer = serializer;   
    }

    // Read
    public async Task RecieveAsync()
    {
        
        string? line;

        try
        {
            while ((line = await _networkService.RecieveAsync()) != null)
            {
                Console.WriteLine("Line recieved");
                
                if (string.IsNullOrWhiteSpace(line)) continue;

                var msg = _serializer.Deserialize<ClientRequest>(line);

                await MessageHandlerAsync(msg!);

            }
        }
        finally
        {
            await DisposeAsync();
        }

        Console.WriteLine("Client disconnected.");



    }

    // Send
    public async Task SendAsync(ServerResponse message)
    {

        Console.WriteLine($"Sending back : {message.Type}" );

        if (message is null) return;

        var json = _serializer.Serialize(message);

        await _networkService.SendAsync(json);

    }

    // client requests
    private async Task MessageHandlerAsync(ClientRequest msg)
    {

        var messageHandler = MessageHandlerFactory.Create(msg.MessageType);

        await messageHandler.HandleMessage(msg, this);
       
        Console.WriteLine("Message received: " + msg.MessageType);
        
        
    }
    

    // equality members
    public override bool Equals(object? obj)
    {
        ClientSessionHandler other = obj as ClientSessionHandler;
        if (other is null)
            return false;

        return Username.Equals(other.Username) && CurrentRoom?.Key.Equals(other.CurrentRoom?.Key) == true;
    
    }

    public static bool operator == (ClientSessionHandler right, ClientSessionHandler left)
    {
        if (ReferenceEquals(right, left))
            return true;

        if ( right is null)
        {
            if (left is null)
                return true;

            return false;
        }

        return right.Equals(left);
    }

    public static bool operator !=(ClientSessionHandler right, ClientSessionHandler left) => !(right == left);

    public override int GetHashCode() => HashCode.Combine(Username, CurrentRoom?.Key);

    public override string ToString()
    {
        return $"Username : {Username} - Endpoint : {EndPoint.Address}:{EndPoint.Port}";
    }

    
    
    async Task CleanUpAsync()
    {
        // stream writer will dispose the underlying stream and the underlying tcp client will dispose the stream as well
        if (CurrentRoom is not null)
        {
            CurrentRoom.RemoveMember(this.Username);
            await CurrentRoom.BroadcastMessageAsync(ServerResponse.UserLeft(this.Username, $"{Username} has left the room"));
        }

        Server.Clients.Remove(this);


    }
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        
        _disposed = true;

        _networkService.Dispose();
        await CleanUpAsync();
        
        GC.SuppressFinalize(this);
    }

    ~ClientSessionHandler()
    {
        DisposeAsync();
    }
}
