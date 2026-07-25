using Diana.Core.Dtos;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Diana.Core.Abstractions.Services;
namespace Diana.Client.Application;




public class VoiceChatService : IVoiceChatService
{
    private UdpClient _udp;

    public event Action<byte[]>? OnVoiceDataRecieved;
    
    public bool IsConnected => _udp?.Client?.Connected ?? false;

    public IPEndPoint EndPoint => (IPEndPoint)_udp?.Client.LocalEndPoint;

    public void Connect(string ipAddress , int port)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);
        
        _udp = new UdpClient(ipAddress, port);
        
        new Thread(Receive) { IsBackground = true }.Start();
        
    }

    public void Disconnect ()
    {
        if (!IsConnected)
            return;

        _udp?.Dispose();
        
    }

   
    public async Task SendAsync( VoiceChatMessage msg)
    {
        if ( !IsConnected ) return;
        // TODO : change the way data transfer from json to bytes ( change the protocol )
        string json = JsonSerializer.Serialize(msg);
        await _udp.SendAsync(Encoding.UTF8.GetBytes(json));
    }

    public void Receive ()
    {
        
        
        try
        {
            IPEndPoint _ = null;

            while (true)
            {
            
                var data = _udp.Receive( ref _ );

                if (data == null) 
                    break;

                OnVoiceDataRecieved?.Invoke(data);

            }
        }
        catch(SocketException) { }
        

    }






}
