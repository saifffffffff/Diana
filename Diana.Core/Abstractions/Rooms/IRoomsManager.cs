using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Core.Abstractions.Rooms;

public interface IRoomsManager
{

    
    IEnumerable<IChatRoom> Rooms { get; }

    bool RemoveRoom(string roomKey);
    (bool Result, string? Error) CreateRoom(string? roomKey, string? roomName, RoomType roomType, ISession roomOwner);
    bool RoomExistsByKey(string key);
    IChatRoom? GetRoomByKey(string key);
}
