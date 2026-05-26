using Diana.Core.Dtos;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Diana.Server;

internal class Server
{

    private readonly int port = 5000;


    public readonly static ConcurrentDictionary< string , ChatRoom> ChatRooms = new ();
    public readonly static LinkedList<ClientHandler> Clients = new ();

    public async Task RunUdpAsync()
    {
        var udp = new UdpClient(port);

        
        while (true)
        {
            
            var result = await udp.ReceiveAsync();
            string json = Encoding.UTF8.GetString(result.Buffer);
            var msg = JsonSerializer.Deserialize<VoiceChatMessage>(json);
            
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
            var client = new ClientHandler(tcp);
            
            Console.WriteLine("Client connected.");

            Clients.AddLast(client);

            Task.Run(async () => await client.Run());

        }

    }

    public static async Task Main ()
    {
        Server server = new Server();
        
        await Task.WhenAll(
            server.RunTcpAsync(),
            server.RunUdpAsync()
        );

    }

    

}
