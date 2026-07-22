using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class MicToggledHandler : IMessageHandler
{


    public async Task HandleMessageAsync(ClientRequest request, ISession clientHandler)
    {


        if (clientHandler.CurrentRoom is null) return;

        bool isMuted = bool.TryParse(request.Content, out var muted) && muted;
        clientHandler.IsMuted = isMuted;

        await clientHandler.CurrentRoom.BroadcastMessageAsync(ServerResponse.MemberMicToggled(clientHandler.Username, isMuted));


    }
}
