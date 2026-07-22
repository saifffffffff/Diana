using Diana.Core.Abstractions.Messaging;
using Diana.Core.Dtos;
using Diana.Server.Messaging.Handlers;
using Microsoft.Extensions.DependencyInjection;
namespace Diana.Server.Messaging;

class MessageHandlerFactory(IServiceProvider serviceProvider) : IMessageHandlerFactory 
{


    public IMessageHandler Create(MessageType messageType)
    {
        return messageType switch
        {
            MessageType.JoinChatRoom => serviceProvider.GetRequiredService<JoinRoomHandler>(),
            MessageType.LeaveRoom => serviceProvider.GetRequiredService < LeaveRoomHandler>(),
            MessageType.SendMessage => serviceProvider.GetRequiredService < SendMessageHandler>(),
            MessageType.GetRoomList => serviceProvider.GetRequiredService < GetRoomListHandler>(),
            MessageType.CreateRoom => serviceProvider.GetRequiredService < CreateRoomHandler>(),
            MessageType.SetUdpEndPoint => serviceProvider.GetRequiredService < SetUdpEndPointHandler>(),
            MessageType.GetVoiceChatRoomMembers => serviceProvider.GetRequiredService < GetVoiceChatRoomMembersHandler>(),
            MessageType.MicToggled => serviceProvider.GetRequiredService < MicToggledHandler>(),
            MessageType.SpeakingStatusChanged => serviceProvider.GetRequiredService <SpeakingStatusChangedHandler>(),
            _ => throw new ArgumentException()
        };
    }
}
