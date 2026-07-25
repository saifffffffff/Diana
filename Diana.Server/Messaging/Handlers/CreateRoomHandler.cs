using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;


class CreateRoomHandler(IRoomsManager roomManager) : IMessageHandler
{

    
    public async Task HandleMessageAsync(ClientRequest request, ISession session  )
    {

        Console.WriteLine("From create room" );
        // TODO : Add Notifier 

        (bool result , string? error ) = roomManager.CreateRoom(request.RoomKey, request.RoomName, request.RoomType, session);

        if ( result)
        {

            
            //var roomsInfo = roomManager.Rooms;
            //foreach (var client in Server.Clients.Where(client => client.CurrentRoom is null))
            //    await client.SendAsync(new ServerResponse
            //    {
            //        Type = MessageType.GetRoomList,
            //        Content = System.Text.Json.JsonSerializer.Serialize(roomsInfo)
            //    });

            await session.SendAsync(ServerResponse.CreateRoomSuccess("Room created successfully"));
        
            Console.WriteLine("room created");
        }

        else
        {
            Console.WriteLine("Room not created");
            await session.SendAsync(ServerResponse.CreateRoomFailed(request.RoomKey, request.RoomName, request.Username, error));
        }



        
    }
}
