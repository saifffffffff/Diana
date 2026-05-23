using Concentus;
using Concentus.Enums;
using Diana.Core.Interfaces;
using System.Runtime.InteropServices;

namespace Diana.Application;

public class OpusAudioCodec : IAudioCodec
{
    private readonly IOpusEncoder _encoder;
    private readonly IOpusDecoder _decoder;

    private const int SampleRate = 48000;
    private const int Channels = 1;
    private const int FrameSize = 960;
    private const int MaxPacket = 4000;
    private const int Bitrate = 32000;

    public OpusAudioCodec()
    {
        _encoder = OpusCodecFactory.CreateEncoder(SampleRate, Channels, OpusApplication.OPUS_APPLICATION_VOIP);
        _encoder.Bitrate = Bitrate;
        _decoder = OpusCodecFactory.CreateDecoder(SampleRate, Channels);
    }

    public byte[] Encode(byte[] pcm)
    {
        // reinterpret byte[] as Span<short> — zero copy, no allocation ✓
        ReadOnlySpan<short> pcmShorts = MemoryMarshal.Cast<byte, short>(pcm);

        byte[] outputBuffer = new byte[MaxPacket];        
        int encodedLength = _encoder.Encode(
            pcmShorts,          
            FrameSize ,
            outputBuffer,
            MaxPacket);

        return outputBuffer[..encodedLength]; // slice 
    }

    public byte[] Decode(byte[] encoded)
    {
        short[] pcmShorts = new short[FrameSize * Channels];

        int decodedSamples = _decoder.Decode(
            encoded.AsSpan(),   
            pcmShorts.AsSpan(),
            FrameSize,
            false);

        // convert decoded shorts back to bytes
        byte[] result = new byte[decodedSamples * 2 * Channels];
        Buffer.BlockCopy(pcmShorts, 0, result, 0, result.Length);

        return result;
    }
}