using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Core.Dtos;

public enum MessageType
{
    // client -> server
    JoinChatRoom,
    JoinVoiceChatRoom,
    LeaveRoom,
    SendMessage,
    CreateRoom,
    GetRoomList,
    SetUdpEndPoint,
    GetVoiceChatRoomMembers,
    MicToggled,
    SpeakingStatusChanged,


    // server -> client
    JoinFailed,
    JoinSuccess,
    SendingMessageFailed,
    RoomCreateFailed,
    RoomCreateSuccess,
    UserLeft,
    UserJoined,
    RoomMemberList,
    MemberMicToggled,
    MemberSpeakingStatusChanged
}
