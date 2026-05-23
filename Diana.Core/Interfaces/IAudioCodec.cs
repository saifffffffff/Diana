
namespace Diana.Core.Interfaces;

public interface IAudioCodec
{

    byte[] Decode(byte[] data);
    byte[] Encode(byte[] data);
}
