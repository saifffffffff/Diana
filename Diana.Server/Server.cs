using Diana.Core.Dtos;
using Diana.Server.Networking;
using Diana.Server.Rooms;
using Diana.Server.Serialization;
using Diana.Server.Sessions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Diana.Server.Serialization;
namespace Diana.Server;

internal class Server
{

    private readonly int port = 5000;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISerializer _serializer;
    public readonly static ConcurrentDictionary<string, ChatRoom> ChatRooms = new();
    public readonly static LinkedList<ClientSessionHandler> Clients = new();

    public Server(IServiceProvider serviceProvider, ISerializer serializer)
    {
        _serviceProvider = serviceProvider;
        _serializer = serializer;
    }

    public async Task RunUdpAsync()
    {
        var udp = new UdpClient(port);

        
        while (true)
        {
            
            var result = await udp.ReceiveAsync();
            string json = Encoding.UTF8.GetString(result.Buffer);
            var msg = System.Text.Json.JsonSerializer.Deserialize<VoiceChatMessage>(json);
            
            if (msg is null) continue;

            var room = ChatRooms[msg.RoomKey];

            //Console.WriteLine(string.Join( ' ' , msg.VoiceData));
              

            foreach (var member in room.Members)
            {

                //if (member.Username == msg.SenderUsername) continue;

                if (member.EndPoint is not null)
                {
                    await udp.SendAsync(result.Buffer, result.Buffer.Length, member.EndPoint);
                }
            }

        }
    }

    public async Task RunTcpAsync()
    {

        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();


        while ( true )
        {
            
            var tcp = await listener.AcceptTcpClientAsync();

            // create a TcpService instance bound to the accepted TcpClient
            var tcpService = ActivatorUtilities.CreateInstance<Networking.TcpService>(_serviceProvider, tcp);

            // create the client session handler; serializer will be resolved from the service provider
            var client = ActivatorUtilities.CreateInstance<ClientSessionHandler>(_serviceProvider, tcpService);

            Console.WriteLine("Client connected.");

            Clients.AddLast(client);

            Task.Run(async () => await client.RecieveAsync());

        }

    }

    public static async Task Main ()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<ISerializer, JsonSerializer>();

        // register per-connection types so they can be created via ActivatorUtilities
        serviceCollection.AddTransient<ClientSessionHandler>();
        serviceCollection.AddTransient<Networking.TcpService>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var serializer = serviceProvider.GetRequiredService<ISerializer>();

        Server server = new Server(serviceProvider, serializer);

        await Task.WhenAll(
            server.RunTcpAsync(),
            server.RunUdpAsync()
        );

    }

    

}
