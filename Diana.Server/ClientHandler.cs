using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Diana.Core.Dtos;
using Diana.Core;
using System.Net;
namespace Diana.Server;

class ClientHandler
{
    public string Username { get; private set; }
    public string ImageBase64 { get; private set; }
    public ChatRoom? CurrentRoom { get; private set; }
    public IPEndPoint EndPoint { get; private set; }

    private readonly NetworkStream _stream;
    private readonly StreamWriter _writer;
    private readonly TcpClient _tcp;

    public ClientHandler(TcpClient tcp)
    {
        _tcp = tcp;
        _stream = tcp.GetStream();
        _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
    }


    public async Task Run()
    {
        StreamReader reader = new StreamReader(_stream);
        string line;

        try
        {
            while ((line = await reader.ReadLineAsync()) != null)
            {
                Console.WriteLine("Line recieved");
                if (string.IsNullOrWhiteSpace(line)) continue;

                var msg = JsonSerializer.Deserialize<ClientRequest>(line);

                await MessageHandlerAsync(msg);

            }
        }
        finally
        {
            await CleanUpAsync();
        }

        Console.WriteLine("Client disconnected.");



    }

    public async Task SendAsync(ServerResponse message)
    {

        Console.WriteLine("Sending back");

        if (message is null) return;

        var json = JsonSerializer.Serialize(message);

        await _writer.WriteLineAsync(json);

    }




    async Task CleanUpAsync()
    {
        // stream writer will dispose the underlying stream and the underlying tcp client will dispose the stream as well
        if (CurrentRoom is not null)
        {
            CurrentRoom.RemoveMember(this.Username);
            await CurrentRoom.BroadcastMessageAsync(ServerResponse.UserLeft(this.Username, $"{Username} has left the room"));

        }

        await _writer.DisposeAsync();

    }

    // client requests
    public async Task MessageHandlerAsync(ClientRequest msg)
    {
        Console.WriteLine("Message received: " + msg.MessageType);

        switch (msg.MessageType)
        {
            case MessageType.JoinChatRoom:
                await HandleJoinRoomRequestAsync(msg);
                break;

            case MessageType.LeaveRoom:
                await HandleLeaveRoomRequestAsync(msg);
                break;

            case MessageType.SendMessage:
                await HandleSendRequestAsync(msg);
                break;

            case MessageType.GetRoomList:
                await HandleRoomListRequestAsync();
                break;

            case MessageType.CreateRoom:
                await HandleCreateRoomRequestAsync(msg);
                break;

            case MessageType.SetUdpEndPoint:
                await HandleSetUdpEndPointRequestAsync(msg);
                break;
            default:
                break;
        }
    }

    private async Task HandleCreateRoomRequestAsync(ClientRequest msg)
    {

        if (string.IsNullOrWhiteSpace(msg.RoomKey) || string.IsNullOrWhiteSpace(msg.RoomName))
        {
            await SendAsync(ServerResponse.CreateRoomFailed(msg.RoomKey, msg.RoomName, msg.Username, "Invalid room key/name"));
            return;
        }

        if (Server.ChatRooms.ContainsKey(msg.RoomKey))
        {
            await SendAsync(ServerResponse.CreateRoomFailed(msg.RoomKey, msg.RoomName, msg.Username, "Room key already exists"));
            return;
        }

        var room = new ChatRoom(msg.RoomKey, msg.RoomName, msg.RoomType, this);

        Server.ChatRooms[room.Key] = room;

        Console.WriteLine("room created");
        await SendAsync(ServerResponse.CreateRoomSuccess("Room created successfully"));



    }

    private async Task HandleLeaveRoomRequestAsync(ClientRequest msg)
    {
        if (CurrentRoom is not null)
        {

            // is he the owner of this room -> move the owner to random person 
            if (CurrentRoom.IsOwner(this))
                CurrentRoom.ChangeOwnerRandomly();



            CurrentRoom.RemoveMember(Username);

            if (CurrentRoom.IsEmpty)
            {
                Console.WriteLine("Room {0} is empty and will be removed.", CurrentRoom.Name); // TODO : send a message to all clients that the room is removed to update the UI
                Server.ChatRooms.TryRemove(CurrentRoom.Key, out _);
            }

            else
                await CurrentRoom.BroadcastMessageAsync(ServerResponse.UserLeft(Username));

            CurrentRoom = null;

        }

    }

    private async Task HandleSendRequestAsync(ClientRequest msg)
    {
        if (CurrentRoom is not null)
            await CurrentRoom.BroadcastMessageAsync(new ServerResponse { Type = MessageType.SendMessage, Content = msg.Content, RoomKey = msg.RoomKey, Username = msg.Username, ImageBase64 = msg.ImageBase64 });

        else
            await SendAsync(ServerResponse.SendingMessageFailed("This user is not joined any room currentry"));

    }

    private async Task HandleJoinRoomRequestAsync(ClientRequest msg)
    {
        string roomKey = msg.RoomKey;

        // invalid key
        if (string.IsNullOrEmpty(roomKey) || !Server.ChatRooms.TryGetValue(roomKey, out var room))
        {
            await SendAsync(ServerResponse.JoinFailed("Invalid room key. Please check and try again."));
            return;
        }

        // validate business rules
        if (room.MemberExistsBy(client => client.Username == msg.Username))
        {
            await SendAsync(
                ServerResponse.JoinFailed($"Username {msg.Username} is already taken in this room !", roomKey)
            );
            return;
        }

        // leave existing room first if the user is already in a room
        if (CurrentRoom is not null)
        {
            CurrentRoom.RemoveMember(Username);
            await CurrentRoom.BroadcastMessageAsync(ServerResponse.UserLeft(Username));
        }


        // this is where i fill the client info
        CurrentRoom = room;
        Username = msg.Username;
        ImageBase64 = msg.ImageBase64;

        room.AddMember(this);

        await SendAsync(ServerResponse.JoinSuccess(roomKey, Username, room.Name, room.Owner.Username));


        await room.BroadcastMessageAsync(ServerResponse.UserJoined(roomKey, this.Username), excludeUsername: this.Username);

    }

    private async Task HandleSetUdpEndPointRequestAsync(ClientRequest msg)
    {
       
        if (msg is null || msg.IpAddress is null )
            return;

        EndPoint = new IPEndPoint(IPAddress.Parse(msg.IpAddress), msg.port);
    }
    
    private async Task HandleRoomListRequestAsync( )
    {
        var roomsInfo = Server.ChatRooms.Values.Select(room => new RoomInfo 
        { 
            RoomName = room.Name,
            RoomKey = room.Key,
            MemberCount = room.MemberCount 
        }).ToList();
        await SendAsync(
            new ServerResponse {
                Type = MessageType.GetRoomList,
                Content = JsonSerializer.Serialize(roomsInfo)
            }
        );
    }

    // equality members
    public override bool Equals(object obj)
    {
        ClientHandler other = obj as ClientHandler;
        if (other is null)
            return false;

        return Username.Equals(other.Username) && CurrentRoom?.Key.Equals(other.CurrentRoom?.Key) == true;
    
    }

    public static bool operator == (ClientHandler right, ClientHandler left)
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

    public static bool operator !=(ClientHandler right, ClientHandler left) => !(right == left);

    public override int GetHashCode() => HashCode.Combine(Username, CurrentRoom?.Key);

    public override string ToString()
    {
        return $"Username : {Username} - Endpoint : {EndPoint.Address}:{EndPoint.Port}";
    }

}
