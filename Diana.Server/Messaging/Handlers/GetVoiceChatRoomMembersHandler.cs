using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
namespace Diana.Server.Messaging.Handlers;

class GetVoiceChatRoomMembersHandler : IMessageHandler
{

    public async Task HandleMessageAsync(ClientRequest request, ISession clientHandler)
    {

        if (clientHandler is null || clientHandler.CurrentRoom is null) return;

        var members = clientHandler.CurrentRoom.GetMembersAsDto();
        await clientHandler.SendAsync(ServerResponse.RoomMemberList(members));
    }
}
