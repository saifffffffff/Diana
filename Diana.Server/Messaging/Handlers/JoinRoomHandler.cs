using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
namespace Diana.Server.Messaging.Handlers;

class JoinRoomHandler( IRoomsManager roomManager ) : IMessageHandler
{
    public async Task HandleMessageAsync(ClientRequest request  , ISession session  )
    {

        var roomKey = request.RoomKey;
        var roomToJoin = roomManager.GetRoomByKey(roomKey);
        
        // invalid key
        if (string.IsNullOrEmpty(roomKey) || roomToJoin == null )
        {
            await session.SendAsync(ServerResponse.JoinFailed("Invalid room key. Please check and try again."));
            return;
        }

        // validate business rules
        if (roomToJoin.MemberExistsBy(client => client.Username == request.Username))
        {
            await session.SendAsync(
                ServerResponse.JoinFailed($"Username \"{request.Username}\" is already taken in this room !", roomKey)
            );
            return;
        }

        // leave existing room first if the user is already in a room
        if (session.CurrentRoom is not null)
        {
            session.CurrentRoom.RemoveMember(session.Username);
            await session.CurrentRoom.BroadcastMessageAsync(ServerResponse.UserLeft(session.Username));
        }


        // this is where i fill the client info
        session.CurrentRoom = roomToJoin;
        session.Username = request.Username;
        session.ImageBase64 = request.ImageBase64;
        
        roomToJoin.AddMember(session);

        await session.SendAsync(ServerResponse.JoinSuccess(roomKey, session.Username, roomToJoin.Name, roomToJoin.Owner.Username, roomToJoin.Type));

        await roomToJoin.BroadcastMessageAsync(ServerResponse.UserJoined(roomKey, session.Username), excludeUsername: session.Username);

    }
}
