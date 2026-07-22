using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;


class CreateRoomHandler(IRoomsManager roomManager) : IMessageHandler
{

    
    public async Task HandleMessageAsync(ClientRequest request, ISession clientHandler  )
    {
        

        // TODO : Add Notifier 
        (bool result , string? error ) = roomManager.CreateRoom(request.RoomKey, request.RoomName, request.RoomType, clientHandler);

        if ( result)
        {
            var roomsInfo = roomManager.Rooms;

            foreach (var client in Server.Clients.Where(client => client.CurrentRoom is null))
                await client.SendAsync(new ServerResponse
                {
                    Type = MessageType.GetRoomList,
                    Content = System.Text.Json.JsonSerializer.Serialize(roomsInfo)
                });

            await clientHandler.SendAsync(ServerResponse.CreateRoomSuccess("Room created successfully"));
        
            Console.WriteLine("room created");
        }

        else
        {
            await clientHandler.SendAsync(ServerResponse.CreateRoomFailed(request.RoomKey, request.RoomName, request.Username, error));
        }



        
    }
}
