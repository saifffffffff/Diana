using Diana.Core.Abstractions.Networking;
using System.Net.Sockets;
using System.Text;
namespace Diana.Server.Networking;

class TcpNetworkChannel : INetworkChannel 
{
    
    private readonly StreamWriter _writer;
    private readonly StreamReader _reader;
    private TcpClient _tcp;
    bool _disposed;


    public TcpNetworkChannel (TcpClient tcp )
    {
        _tcp = tcp;
        _writer = new StreamWriter(_tcp.GetStream(), Encoding.UTF8) { AutoFlush = true };
        _reader = new StreamReader(_tcp.GetStream());
    }

    public async Task<string?> RecieveAsync()
    {
        return await _reader.ReadLineAsync() ;
    }

    public async Task SendAsync(string message)
    {
        await _writer.WriteLineAsync(message);
    }

    
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _writer.Dispose();
        _reader.Dispose();
        _tcp.Dispose();

        GC.SuppressFinalize(this);
    }

    
            
        
    

    ~TcpNetworkChannel()
    {
        Dispose();
    }

}
