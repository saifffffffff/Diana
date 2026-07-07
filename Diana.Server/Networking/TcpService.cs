using System.Net.Sockets;
using System.Text;
namespace Diana.Server.Networking;

class TcpService : INetworkService 
{
    
    private readonly StreamWriter _writer;
    private readonly StreamReader _reader;
    private readonly TcpClient _tcp;
    bool _disposed;


    public TcpService (TcpClient tcp )
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

    ~TcpService()
    {
        Dispose();
    }

}
