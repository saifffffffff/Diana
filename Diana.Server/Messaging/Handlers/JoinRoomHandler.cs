using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
namespace Diana.Server.Messaging.Handlers;

class JoinRoomHandler( IRoomsManager roomManager ) : IMessageHandler
{
    public async Task HandleMessageAsync(ClientRequest msg  , ISession clientHandler  )
    {
        var roomKey = msg.RoomKey;
        var roomToJoin = roomManager.GetRoomByKey(roomKey);
        // invalid key
        if (string.IsNullOrEmpty(roomKey) || roomToJoin != null )
        {
            await clientHandler.SendAsync(ServerResponse.JoinFailed("Invalid room key. Please check and try again."));
            return;
        }

        // validate business rules
        if (roomToJoin.MemberExistsBy(client => client.Username == msg.Username))
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
        clientHandler.CurrentRoom = roomToJoin;
        clientHandler.Username = msg.Username;
        clientHandler.ImageBase64 = msg.ImageBase64;

        roomToJoin.AddMember(clientHandler);

        await clientHandler.SendAsync(ServerResponse.JoinSuccess(roomKey, clientHandler.Username, roomToJoin.Name, roomToJoin.Owner.Username, roomToJoin.Type));

        await roomToJoin.BroadcastMessageAsync(ServerResponse.UserJoined(roomKey, clientHandler.Username), excludeUsername: clientHandler.Username);

    }
}
