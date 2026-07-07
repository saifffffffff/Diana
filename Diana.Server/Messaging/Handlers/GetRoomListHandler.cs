using Diana.Core.Dtos;
using Diana.Server.Rooms;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class GetRoomListHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg, ClientSessionHandler clientHandler)
    {
        var roomsInfo = ChatRoom.GetRoomsAsRoomInfo();

        await clientHandler.SendAsync(
            new ServerResponse
            {
                Type = MessageType.GetRoomList,
                Content = roomsInfo
            }
        );

    }
}
