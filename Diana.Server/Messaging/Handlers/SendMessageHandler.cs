using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class SendMessageHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg, ClientSessionHandler clientHandler)
    {
        if (clientHandler.CurrentRoom is not null)
            await clientHandler.CurrentRoom.BroadcastMessageAsync(
                new ServerResponse
                {
                    Type = MessageType.SendMessage,
                    Content = msg.Content,
                    RoomKey = msg.RoomKey,
                    Username = msg.Username,
                    ImageBase64 = clientHandler.ImageBase64
                });
        else
            await clientHandler.SendAsync(ServerResponse.SendingMessageFailed("This user is not joined any room currently"));
    }
}
