namespace Diana.Core.Abstractions.Services;

public interface IAudioCodec
{

    byte[] Decode(byte[] data);
    byte[] Encode(byte[] data);
}
