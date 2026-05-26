using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Core.Dtos;

public class ServerResponse
{
    public string Content { get; set; }

    public string Username { get; set; }

    public string RoomKey { get; set; }

    public string RoomName { get; set; }

    public string ImageBase64 { get; set; }

    public string RoomOwner { get; private set; }

    public RoomType RoomType { get; set; }

    public DateTime TimeStamp { get; set; } = DateTime.Now;

    public MessageType Type { get; set; }

    public static ServerResponse UserLeft(string username, string content = null) =>
        new ServerResponse
        {
            Type = MessageType.UserLeft,
            Username = username,
            Content = content ?? $"{username} has left the room"
        };

    public static ServerResponse UserJoined(string roomKey, string username, string content = null) =>
        new ServerResponse
        {
            Type = MessageType.UserJoined,
            RoomKey = roomKey,
            Username = username,
            Content = content ?? $"{username} has joined the room"
        };

    public static ServerResponse JoinFailed(string content, string roomKey = null) =>
        new ServerResponse
        {
            Type = MessageType.JoinFailed,
            Content = content,
            RoomKey = roomKey
        };

    public static ServerResponse JoinSuccess(string roomKey, string username, string roomName, string OwnerName, RoomType roomType = RoomType.Voice) =>
        new ServerResponse
        {
            Type = MessageType.JoinSuccess,
            RoomKey = roomKey,
            Username = username,
            RoomName = roomName,
            RoomOwner = OwnerName,
            RoomType = roomType
        };

    public static ServerResponse SendingMessageFailed( string content) =>
        new ServerResponse
        {
            Type = MessageType.SendingMessageFailed,
            Content = content
        };

    public static ServerResponse CreateRoomFailed(string roomKey, string roomName, string username, string content) =>
        new ServerResponse { Type = MessageType.RoomCreateFailed, RoomName = roomName, RoomKey = roomKey, Content = content };

    public static ServerResponse CreateRoomSuccess(string content) => new ServerResponse { Type = MessageType.RoomCreateSuccess, Content = content };

    public static ServerResponse RoomMemberList(IEnumerable<VoiceChatRoomMember> members)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(members);
        return new ServerResponse { Type = MessageType.RoomMemberList, Content = json };
    }

    public static ServerResponse MemberMicToggled(string username, bool isMuted) =>
        new ServerResponse { Type = MessageType.MemberMicToggled, Username = username, Content = isMuted.ToString() };

    public static ServerResponse MemberSpeakingStatusChanged(string username, bool isSpeaking) =>
        new ServerResponse { Type = MessageType.MemberSpeakingStatusChanged, Username = username, Content = isSpeaking.ToString() };
}

