using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class MicToggledHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg, ClientSessionHandler clientHandler)
    {
        
        if (clientHandler.CurrentRoom is null) return;

        bool isMuted = bool.TryParse(msg.Content, out var muted) && muted;
        clientHandler.IsMuted = isMuted;

        await clientHandler.CurrentRoom.BroadcastMessageAsync(ServerResponse.MemberMicToggled(clientHandler.Username, isMuted));

    }
}
