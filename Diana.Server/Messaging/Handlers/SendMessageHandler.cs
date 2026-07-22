using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class SendMessageHandler : IMessageHandler
{

   
    public async Task HandleMessageAsync(ClientRequest request, ISession clientHandler)
    {
        if (clientHandler.CurrentRoom is not null)
            await clientHandler.CurrentRoom.BroadcastMessageAsync(
                new ServerResponse
                {
                    Type = MessageType.SendMessage,
                    Content = request.Content,
                    RoomKey = request.RoomKey,
                    Username = request.Username,
                    ImageBase64 = clientHandler.ImageBase64
                });
        else
            await clientHandler.SendAsync(ServerResponse.SendingMessageFailed("This user is not joined any room currently"));
    }
}
