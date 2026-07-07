using Diana.Core.Dtos;
using Diana.Core;
using System.Net.Sockets;
using Diana.Server.Sessions;

namespace Diana.Server.Rooms;




class ChatRoom
{
    public ClientSessionHandler Owner { get; set; }
    
    public string Key { get; }
    public string Name { get; }
    public RoomType Type { get; }
    public int MemberCount => _members.Count;

    private Dictionary< string , ClientSessionHandler> _members { get; } = new ();

    public IEnumerable<ClientSessionHandler> Members => _members.Values;

    public ChatRoom(string key , string name , RoomType type, ClientSessionHandler owner ) 
    {
        this.Key = key;
        this.Name = name;
        this.Owner = owner;
        this.Type = type;
    }

    // tcp
    public async Task BroadcastMessageAsync ( ServerResponse msg , string excludeUsername = null)
    {
        foreach (var member in _members.Values)
        {
            if (excludeUsername is not null && excludeUsername == member.Username) continue;

            await member.SendAsync(msg);
        }
        
    }

   

    public bool IsEmpty => _members.Count == 0;

    public bool IsOwner(ClientSessionHandler client) => client.Username == Owner.Username; // TODO : make the comparison better because 

    public void AddMember(ClientSessionHandler client)
    {
        ArgumentNullException.ThrowIfNull(client);

        _members[client.Username] = client;
    }
    
    public void RemoveMember(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        _members.Remove(username);
    }

    public void ChangeOwnerRandomly()
    {
        if ( _members.Count <= 1)
            return;
        
        Owner = _members.Values.First( member => member != Owner );
    
    }

    public bool MemberExistsBy(Func<ClientSessionHandler, bool> predicate) => _members.Values.Any(predicate);

    public IEnumerable<VoiceChatRoomMember> GetMembersAsDto()
    {
        return _members.Values.Select(m => new VoiceChatRoomMember(
            m.Username,
            m.ImageBase64 ?? string.Empty,
            m.IsMuted,
            m.IsSpeaking
        ));
    }

    public static IEnumerable<RoomInfo> GetRoomsAsRoomInfo() => Server.ChatRooms.Values.Select(room => new RoomInfo
    {
        RoomName = room.Name,
        RoomKey = room.Key,
        MemberCount = room.MemberCount,
        RoomType = room.Type
    }).ToList();

}
