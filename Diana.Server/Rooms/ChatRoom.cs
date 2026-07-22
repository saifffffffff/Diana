using Diana.Core.Dtos;
using Diana.Core;
using System.Net.Sockets;
using Diana.Core.Abstractions.Sessions;
using Diana.Server.Sessions;
using Diana.Core.Abstractions.Rooms;
using System.Collections.Concurrent;
namespace Diana.Server.Rooms;



// make sure this follow single responslibity
class ChatRoom : IChatRoom
{
    public ISession Owner { get; set; }
    
    public string Key { get; }
    public string Name { get; }
    public RoomType Type { get; }
    public bool IsEmpty => _members.Count == 0;
    public int MemberCount => _members.Count;
    readonly Dictionary< string , ISession> _members  = new ();
    
    public IEnumerable<ISession> Members => _members.Values;

    public ChatRoom(string key , string name , RoomType type, ISession owner ) 
    {
        this.Key = key;
        this.Name = name;
        this.Owner = owner;
        this.Type = type;
    }

    public async Task BroadcastMessageAsync ( ServerResponse msg , string? excludeUsername = null)
    {
        foreach (var member in _members.Values)
        {
            if (excludeUsername is not null && excludeUsername == member.Username) continue;

            await member.SendAsync(msg);
        }
        
    }

    public void AddMember(ISession client)
    {
        ArgumentNullException.ThrowIfNull(client);
   
        _members[client.Username!] = client;
    }
    
    public bool RemoveMember(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return _members.Remove(username);
    }

    public void ChangeOwnerRandomly()
    {
        if ( _members.Count <= 1)
            return;
        
        Owner = _members.Values.First( member => member != Owner );
    
    }

    public bool MemberExistsBy(Func<ISession, bool> predicate) => _members.Values.Any(predicate);

    public IEnumerable<VoiceChatRoomMember> GetMembersAsDto()
    {
        return _members.Values.Select(m => new VoiceChatRoomMember(
            m.Username,
            m.ImageBase64 ?? string.Empty,
            m.IsMuted,
            m.IsSpeaking
        ));
    }


    
}

    //public static IEnumerable<RoomInfo> GetRoomsAsRoomInfo() => Server.ChatRooms.Values.Select(room => new RoomInfo
    //{
    //    RoomName = room.Name,
    //    RoomKey = room.Key,
    //    MemberCount = room.MemberCount,
    //    RoomType = room.Type
    //}).ToList();