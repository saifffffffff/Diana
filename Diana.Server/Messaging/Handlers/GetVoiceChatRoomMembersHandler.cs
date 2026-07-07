using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class GetVoiceChatRoomMembersHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg, ClientSessionHandler clientHandler)
    {
        if (clientHandler.CurrentRoom is null) return;

        var members = clientHandler.CurrentRoom.GetMembersAsDto();
        await clientHandler.SendAsync(ServerResponse.RoomMemberList(members));
    }
}
