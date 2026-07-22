using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class SpeakingStatusChangedHandler : IMessageHandler
{
    public async Task HandleMessageAsync(ClientRequest request, ISession clientHandler  )
    {
        if (clientHandler.CurrentRoom is null) return;

        bool isSpeaking = bool.TryParse(request.Content, out var speaking) && speaking;
        clientHandler.IsSpeaking = isSpeaking;

        await clientHandler.CurrentRoom.BroadcastMessageAsync(ServerResponse.MemberSpeakingStatusChanged(clientHandler.Username, isSpeaking));

    }

    
}
