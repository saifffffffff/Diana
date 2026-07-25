using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Diana.Core.Dtos;

public class ClientRequest
{
    public string? Content { get; set; }

    public string? Username { get; set; }

    public string? RoomKey { get; set; }

    public string? RoomName { get; set; }

    public string? ImageBase64 { get; set; }

    public MessageType MessageType { get; set; }
    
    public RoomType RoomType { get;  set; }
    
    public string? IpAddress { get;  set; }

    public int port { get;  set; }


    public static ClientRequest LeaveRoom(string roomKey, string username) =>
        new ClientRequest { MessageType = MessageType.LeaveRoom, RoomKey = roomKey, Username = username };

    public static ClientRequest JoinRoom(string roomKey, string username, string imageBase64 = null) =>
        new ClientRequest { MessageType = MessageType.JoinChatRoom, RoomKey = roomKey, Username = username, ImageBase64 = imageBase64 };
    
    public static ClientRequest SendMessage(string roomKey, string username, string content, string imageBase64 = null) =>
        new ClientRequest { MessageType = MessageType.SendMessage, Username = username, Content = content, RoomKey = roomKey };

    public static ClientRequest CreateRoom(string roomKey, string roomName, string username, string content , RoomType roomType) =>
        new ClientRequest { MessageType = MessageType.CreateRoom, Content = content, RoomKey = roomKey, RoomName = roomName, Username = username , RoomType = roomType };

    public static ClientRequest GetRoomList() => new ClientRequest { MessageType = MessageType.GetRoomList };

    public static ClientRequest SetUdpEndPoint(IPEndPoint endPoint) {
        
        return new ClientRequest { IpAddress = endPoint.Address.ToString() , port = endPoint.Port , MessageType = MessageType.SetUdpEndPoint};
    }

    public static ClientRequest GetVoiceChatRoomMembers(string roomKey) =>
        new ClientRequest { MessageType = MessageType.GetVoiceChatRoomMembers, RoomKey = roomKey };

    public static ClientRequest MicToggled(string roomKey, string username, bool isMuted) =>
        new ClientRequest { MessageType = MessageType.MicToggled, RoomKey = roomKey, Username = username, Content = isMuted.ToString() };

    public static ClientRequest SpeakingStatusChanged(string roomKey, string username, bool isSpeaking) =>
        new ClientRequest { MessageType = MessageType.SpeakingStatusChanged, RoomKey = roomKey, Username = username, Content = isSpeaking.ToString() };

    
    public override string ToString()
    {
        // Truncate ImageBase64 to keep the log output clean and readable
        string imageDisplay = string.IsNullOrEmpty(ImageBase64)
            ? "null"
            : $"[Base64 String, Length: {ImageBase64.Length}]";

        return $"Message [" +
               $"Type: {MessageType}, " +
               $"Username: '{Username ?? "null"}', " +
               $"Content: '{Content ?? "null"}', " +
               $"RoomName: '{RoomName ?? "null"}', " +
               $"RoomKey: '{RoomKey ?? "null"}', " +
               $"RoomType: {RoomType}, " +
               $"IP: {IpAddress ?? "null"}:{port}, " +
               $"Image: {imageDisplay}" +
               $"]";
    }
}


