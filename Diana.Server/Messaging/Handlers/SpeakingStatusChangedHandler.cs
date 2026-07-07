using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class SpeakingStatusChangedHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg, ClientSessionHandler clientHandler)
    {
        if (clientHandler.CurrentRoom is null) return;

        bool isSpeaking = bool.TryParse(msg.Content, out var speaking) && speaking;
        clientHandler.IsSpeaking = isSpeaking;

        await clientHandler.CurrentRoom.BroadcastMessageAsync(ServerResponse.MemberSpeakingStatusChanged(clientHandler.Username, isSpeaking));

    }
}
