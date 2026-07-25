using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;

using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class GetRoomListHandler (IRoomsManager roomManager ) : IMessageHandler
{
    
    public async Task HandleMessageAsync(ClientRequest request, ISession clientHandler  )
    {    

        await clientHandler.SendAsync(
            new ServerResponse
            {
                Type = MessageType.GetRoomList,
                Content = roomManager.Rooms.Select  ( room => new RoomInfo { RoomName = room.Name , OwnerName = room.Owner.Username , RoomKey = room.Key , RoomType = room.Type , MemberCount = room.MemberCount})
            }
        );
    }

    
}
