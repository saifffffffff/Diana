using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class JoinRoomRequestHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg  , ClientSessionHandler clientHandler)
    {
        string roomKey = msg.RoomKey;

        // invalid key
        if (string.IsNullOrEmpty(roomKey) || !Server.ChatRooms.TryGetValue(roomKey, out var room))
        {
            await clientHandler.SendAsync(ServerResponse.JoinFailed("Invalid room key. Please check and try again."));
            return;
        }

        // validate business rules
        if (room.MemberExistsBy(client => client.Username == msg.Username))
        {
            await clientHandler.SendAsync(
                ServerResponse.JoinFailed($"Username {msg.Username} is already taken in this room !", roomKey)
            );
            return;
        }

        // leave existing room first if the user is already in a room
        if (clientHandler.CurrentRoom is not null)
        {
            clientHandler.CurrentRoom.RemoveMember(clientHandler.Username);
            await clientHandler.CurrentRoom.BroadcastMessageAsync(ServerResponse.UserLeft(clientHandler.Username));
        }


        // this is where i fill the client info
        clientHandler.CurrentRoom = room;
        clientHandler.Username = msg.Username;
        clientHandler.ImageBase64 = msg.ImageBase64;

        room.AddMember(clientHandler);

        await clientHandler.SendAsync(ServerResponse.JoinSuccess(roomKey, clientHandler.Username, room.Name, room.Owner.Username, room.Type));

        await room.BroadcastMessageAsync(ServerResponse.UserJoined(roomKey, clientHandler.Username), excludeUsername: clientHandler.Username);

    }
}
