using Diana.Core.Dtos;
using Diana.Server.Rooms;
using Diana.Server.Sessions;
namespace Diana.Server.Messaging.Handlers;

class CreateRoomHandler : IMessageHandler
{
    public async Task HandleMessage(ClientRequest msg, ClientSessionHandler clientHandler)
    {
        if (string.IsNullOrWhiteSpace(msg.RoomKey) || string.IsNullOrWhiteSpace(msg.RoomName))
        {
            await clientHandler.SendAsync(ServerResponse.CreateRoomFailed(msg.RoomKey, msg.RoomName, msg.Username, "Invalid room key/name"));
            return;
        }

        if (Server.ChatRooms.ContainsKey(msg.RoomKey))
        {
            await clientHandler.SendAsync(ServerResponse.CreateRoomFailed(msg.RoomKey, msg.RoomName, msg.Username, "Room key already exists"));
            return;
        }

        var room = new ChatRoom(msg.RoomKey, msg.RoomName, msg.RoomType, clientHandler);

        Server.ChatRooms[room.Key] = room;

        Console.WriteLine("room created");

        var roomsInfo = ChatRoom.GetRoomsAsRoomInfo();

        foreach (var client in Server.Clients.Where(client => client.CurrentRoom is null))
            await client.SendAsync(new ServerResponse
            {
                Type = MessageType.GetRoomList,
                Content = System.Text.Json.JsonSerializer.Serialize(roomsInfo)
            });

        await clientHandler.SendAsync(ServerResponse.CreateRoomSuccess("Room created successfully"));

    }
}
