using Diana.Core;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Sessions;
using Diana.Server.Networking;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Server.Rooms;

class RoomManager : IRoomsManager
{
    IServiceProvider _serviceProvider;
    Dictionary<string, IChatRoom> _rooms = new();
    public IEnumerable<IChatRoom> Rooms { get => _rooms.Values; }

    public RoomManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public (bool Result , string? Error ) CreateRoom(string? roomKey, string? roomName, RoomType roomType, ISession roomOwner)
    {
        // business rules 1 - room key is uniqe
        // owner can not be joined to other

        if (roomName is null || roomKey is null )
            return ( false , "Invalid room key/name");
        

        foreach (var room in _rooms.Values)
            if ( room.Members.Contains(roomOwner))
                return ( false , "user already joined another room");
            

        if (_rooms.ContainsKey(roomKey))
            return (false, "Room key already exists");

        // TODO : make a room Factory
        //var createdRoom = ActivatorUtilities.CreateInstance<ChatRoom>(_serviceProvider, roomKey , roomName , roomType , roomOwner);
        var createdRoom = new ChatRoom(roomKey, roomName, roomType, roomOwner);
        _rooms.Add(roomKey, createdRoom);

        return (true, null ); 
        
    }

    
    public IChatRoom? GetRoomByKey(string key)
    {
        return _rooms.GetValueOrDefault(key);
    }

    
    public bool RemoveRoom(string roomKey)
    {
        return _rooms.Remove(roomKey);
    }

    public bool RoomExistsByKey(string key)
    {
        return GetRoomByKey(key) != null ;
    }
}
