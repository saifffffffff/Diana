using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Networking;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Serialization;
using Diana.Core.Abstractions.Sessions;
using Diana.Server.Messaging;
using Diana.Server.Messaging.Handlers;
using Diana.Server.Rooms;
using Diana.Server.Sessions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;


namespace Diana.Server.Networking;


class Program
{
    public static async Task Main()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<ISerializer, JsonSerializer>();
        serviceCollection.AddSingleton<IRoomsManager, RoomManager>();
        serviceCollection.AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>();

        // Register handlers (each can have different dependencies!)
        serviceCollection.AddSingleton<JoinRoomHandler>();
        serviceCollection.AddSingleton<LeaveRoomHandler>();
        serviceCollection.AddSingleton<SendMessageHandler>();
        serviceCollection.AddSingleton<GetRoomListHandler>();
        serviceCollection.AddSingleton<CreateRoomHandler>();
        serviceCollection.AddSingleton<SetUdpEndPointHandler>();
        serviceCollection.AddSingleton<GetVoiceChatRoomMembersHandler>();
        serviceCollection.AddSingleton<MicToggledHandler>();
        serviceCollection.AddSingleton<SpeakingStatusChangedHandler>();

        // register per-connection types so they can be created via ActivatorUtilities
        serviceCollection.AddTransient<ISession, Session>();
        serviceCollection.AddTransient<INetworkChannel, TcpNetworkChannel>();
        serviceCollection.AddTransient<IChatRoom, ChatRoom>();
        serviceCollection.AddTransient<IConnectionAcceptor, TcpConnectionAcceptor>();
        serviceCollection.AddTransient<IVoiceStreamRelay, UdpVoiceStreamRelay>();
        // ( runtime parameter ) => service instance

        // network channel factory
        serviceCollection.AddTransient<Func<TcpClient, INetworkChannel>>(serviceProvider => // parameter represents the implementation of that function
        {
            return tcpClient => ActivatorUtilities.CreateInstance<TcpNetworkChannel>(serviceProvider, tcpClient);
        });


        // session factory 
        serviceCollection.AddTransient<Func<INetworkChannel, ISession>>(serviceProvider =>
        {
            return networkChannel => ActivatorUtilities.CreateInstance<Session>(serviceProvider, networkChannel);
        });


        var serviceProvider = serviceCollection.BuildServiceProvider();


        var connectionAcceptor = serviceProvider.GetRequiredService<IConnectionAcceptor>();
        var voiceRalay = serviceProvider.GetRequiredService<IVoiceStreamRelay>();


        await Task.WhenAll(
            connectionAcceptor.RunAsync(),
            voiceRalay.RunAsync()
        );
        
        
    }
}



