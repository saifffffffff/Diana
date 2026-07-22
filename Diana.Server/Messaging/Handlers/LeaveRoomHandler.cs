using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using Diana.Server.Sessions;

namespace Diana.Server.Messaging.Handlers;

class LeaveRoomHandler(IRoomsManager roomManager , IMessageHandlerFactory messageHandlerFactory) : IMessageHandler
{
   
    public async Task HandleMessageAsync(ClientRequest request, ISession clientHandler)
    {
        

        if (clientHandler!.CurrentRoom is not null)
        {
            // is he the owner of this room -> move the owner to random person 
            if (clientHandler.CurrentRoom.Owner == clientHandler)
                clientHandler.CurrentRoom.ChangeOwnerRandomly();

            clientHandler.CurrentRoom.RemoveMember(clientHandler.Username!);

            if (clientHandler.CurrentRoom.IsEmpty)
            {
                Console.WriteLine("Room {0} is empty and will be removed.", clientHandler.CurrentRoom.Name);
                roomManager.RemoveRoom(clientHandler.CurrentRoom.Key);

                // TODO : change this with notification service
                foreach (var client in Server.Clients)
                    await messageHandlerFactory.Create(MessageType.GetRoomList).HandleMessageAsync(request, client);
            }
            else
                await clientHandler.CurrentRoom.BroadcastMessageAsync(ServerResponse.UserLeft(clientHandler.Username));

            clientHandler.IsSpeaking = false;
            clientHandler.IsMuted = false;
            clientHandler.CurrentRoom = null;
        }
    }
}
