using Diana.Core.Dtos;
using Diana.Server.Messaging.Handlers;
namespace Diana.Server.Messaging;

class MessageHandlerFactory
{
    public static IMessageHandler Create(MessageType messageType)
    {
        switch (messageType)
        {
            case MessageType.JoinChatRoom:
                return new JoinRoomRequestHandler();

            case MessageType.LeaveRoom:
                return new LeaveRoomHandler();

            case MessageType.SendMessage:
                return new SendMessageHandler();

            case MessageType.GetRoomList:
                return new GetRoomListHandler();

            case MessageType.CreateRoom:
                return new CreateRoomHandler();

            case MessageType.SetUdpEndPoint:
                return new SetUdpEndPointHandler();

            case MessageType.GetVoiceChatRoomMembers:
                return new GetVoiceChatRoomMembersHandler();

            case MessageType.MicToggled:
                return new MicToggledHandler();

            case MessageType.SpeakingStatusChanged:
                return new SpeakingStatusChangedHandler();

            default:
                return null;
        }
    }
}
