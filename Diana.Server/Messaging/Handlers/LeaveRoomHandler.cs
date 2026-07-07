using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class LeaveRoomHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg  , ClientSessionHandler clientHanlder)
    {
        if (clientHanlder.CurrentRoom is not null)
        {

            // is he the owner of this room -> move the owner to random person 
            if (clientHanlder.CurrentRoom.IsOwner(clientHanlder))
                clientHanlder.CurrentRoom.ChangeOwnerRandomly();



            clientHanlder.CurrentRoom.RemoveMember(clientHanlder.Username);

            if (clientHanlder.CurrentRoom.IsEmpty)
            {
                Console.WriteLine("Room {0} is empty and will be removed.", clientHanlder.CurrentRoom.Name); // TODO : send a message to all clients that the room is removed to update the UI
                Server.ChatRooms.TryRemove(clientHanlder.CurrentRoom.Key, out _);

                foreach (var client in Server.Clients)
                    await MessageHandlerFactory.Create(MessageType.GetRoomList).HandleMessage(msg, clientHanlder);
            }

            else
                await clientHanlder.CurrentRoom.BroadcastMessageAsync(ServerResponse.UserLeft(clientHanlder.Username));

            clientHanlder.IsSpeaking = false;
            clientHanlder.IsMuted = false;
            clientHanlder.CurrentRoom = null;

        }
    }
}
