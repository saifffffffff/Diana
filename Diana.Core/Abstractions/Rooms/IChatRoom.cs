using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Core.Abstractions.Rooms;

public interface IChatRoom
{
    string Key { get; }
    string Name { get; }
    bool IsEmpty { get; }
    RoomType Type { get; }
    ISession Owner { get; set; }  
    IEnumerable<ISession> Members { get; }



    void AddMember( ISession client);
    bool RemoveMember(string username);
    void ChangeOwnerRandomly();
    Task BroadcastMessageAsync(ServerResponse msg, string? excludeUsername = null);
   


    // Queries 
    public bool MemberExistsBy(Func<ISession, bool> predicate);
    public IEnumerable<VoiceChatRoomMember> GetMembersAsDto();


    
    

}


