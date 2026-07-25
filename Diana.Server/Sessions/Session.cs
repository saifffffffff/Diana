using System.Net.WebSockets;
using System.Text.Json;
using Diana.Core.Dtos;
using Diana.Core;
using System.Net;
using Diana.Core.Abstractions.Serialization;
using Diana.Core.Abstractions.Networking;
using Diana.Core.Abstractions.Sessions;
using Diana.Server.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Messaging;

namespace Diana.Server.Sessions;

// TODO : Solve encapsulation issue : private setters + setters methods
// managing the lifecycle of one connected client's session
class Session(INetworkChannel networkService, ISerializer serializer, IMessageHandlerFactory messageHandlersFactory) : ISession
{
    public string? Username { get; set; }
    public string? ImageBase64 { get; set; }
    public IPEndPoint? EndPoint { get;  set; }
    public bool IsMuted { get; set; }
    public bool IsSpeaking { get; set; }
    public IChatRoom? CurrentRoom { get; set ; }

    bool _disposed;


    // Read
    public async Task RecieveAsync()
    {
        
        string? line;

        try
        {
            while ((line = await networkService.RecieveAsync()) != null)
            {
                
                if (string.IsNullOrWhiteSpace(line)) continue;

                var request = serializer.Deserialize<ClientRequest>(line);
                
                Console.WriteLine("request received: " + request);

                await HandleRequestAsync(request!);

            }
        }
        finally
        {
            await DisposeAsync();
        }

        Console.WriteLine("Client disconnected.");



    }

    // Send
    public async Task SendAsync(ServerResponse response)
    {

        Console.WriteLine($"Sending response : {response.Type}\n" );

        if (response is null) return;
        
        try
        {
            var json = serializer.Serialize(response);

            await networkService.SendAsync(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    // client requests
    public async Task HandleRequestAsync(ClientRequest request)
    {

        var messageHandler = messageHandlersFactory.Create(request.MessageType);

        await messageHandler.HandleMessageAsync(request, this);
       
        
        
    }
    

    // equality members
    public override bool Equals(object? obj)
    {
        Session other = obj as Session;
     
        if (other is null)
            return false;

        return Username.Equals(other.Username) && CurrentRoom?.Key.Equals(other.CurrentRoom?.Key) == true;
    
    }

    public static bool operator == (Session right, Session left)
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

    public static bool operator !=(Session right, Session left) => !(right == left);

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

    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;
        
        _disposed = true;

        networkService.Dispose();
        await CleanUpAsync();
        
        GC.SuppressFinalize(this);
    }
    ~Session()
    {
        DisposeAsync();
    }
}
