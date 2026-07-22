using Diana.Core.Abstractions.Messaging;
using Diana.Core.Abstractions.Networking;
using Diana.Core.Abstractions.Rooms;
using Diana.Core.Abstractions.Serialization;
using Diana.Core.Abstractions.Sessions;
using Diana.Core.Dtos;
using Diana.Server.Messaging;
using Diana.Server.Messaging.Handlers;
using Diana.Server.Networking;
using Diana.Server.Rooms;
using Diana.Server.Sessions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Diana.Server;

internal class Server(ISerializer serializer , IServiceProvider serviceProvider , IRoomsManager roomManager)
{

    private readonly int port = 5000;
    
    public readonly static LinkedList<ISession> Clients = new();
    

    public async Task RunUdpAsync()
    {
        var udp = new UdpClient(port);

        
        while (true)
        {
            
            var result = await udp.ReceiveAsync();
            string msgStr = Encoding.UTF8.GetString(result.Buffer);
            var msg = serializer.Deserialize<VoiceChatMessage>(msgStr);
            
            if (msg is null) continue;

            var room = roomManager.GetRoomByKey(msg.RoomKey);

            //Console.WriteLine(string.Join( ' ' , msg.VoiceData));
              

            foreach (var member in room.Members)
            {

                if (member.Username == msg.SenderUsername) continue;

                if (member.EndPoint is not null)
                {
                    await udp.SendAsync(result.Buffer, result.Buffer.Length, member.EndPoint);
                }
            }

        }
    }

    public async Task RunTcpAsync()
    {
        // recieve connection -> create a client handler 
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        while ( true )
        {
            
            var tcp = await listener.AcceptTcpClientAsync();

            // create a TcpService instance bound to the accepted TcpClient
            var networkService = ActivatorUtilities.CreateInstance<INetworkService>(serviceProvider, tcp);

            // create the client session handler; serializer will be resolved from the service provider
            var client = ActivatorUtilities.CreateInstance<ISession>(serviceProvider, networkService);

            Console.WriteLine("Client connected.");

            Clients.AddLast(client);

            Task.Run(async () => await client.RecieveAsync());

        }

    }

    public static async Task Main ()
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
        serviceCollection.AddTransient< ISession, Session>();
        serviceCollection.AddTransient< INetworkService , TcpService>();
        serviceCollection.AddTransient<IChatRoom, ChatRoom>();
        serviceCollection.AddTransient<Server>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        Server server = serviceProvider.GetRequiredService<Server>();

        await Task.WhenAll(
            server.RunTcpAsync(),
            server.RunUdpAsync()
        );

    }

    

}