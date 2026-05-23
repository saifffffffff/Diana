

using Diana.Core.Dtos;
using Diana.Core.Interfaces;
using System.Text;
using System.Text.Json;
namespace Diana.Presenter;

public class VoiceChatRoomPresenter
{

    private readonly IVoiceChatRoomView _view;
    private readonly IVoiceChatService _voiceChatService;
    private readonly IAudioCodec _audioCodec;

    public VoiceChatRoomPresenter( IVoiceChatRoomView view , IVoiceChatService voiceChatService ,IAudioCodec audioCodec)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(voiceChatService);
        ArgumentNullException.ThrowIfNull(audioCodec);
        
        _view = view;
        _audioCodec = audioCodec;
        _voiceChatService = voiceChatService;
        //_voiceChatService.Connect("127.0.0.1", 5000);
        _voiceChatService.OnVoiceDataRecieved += OnDataRecieved;
       
        _view.OnCopyRoomKeyPressed += this.OnCopyRoomKeyPressed;
        _view.OnLeaveRoomPressed += this.OnLeaveRoomPressed;
        _view.OnSettingsPressed += this.OnSettingsPressed; 
        _view.OnMicrophoneTogglePressed += OnMicrophoneTogglePressed;
        _view.OnDataFromMicrophoneReceived += async (data) => await OnDataFromMicrophoneReceived(data);
        
        _view.IsMicrophoneActive = true;
       


    }

    void OnCopyRoomKeyPressed()
    {
        _view.CopyRoomKeyToClipboard();
    }

    void OnLeaveRoomPressed()
    {
        _voiceChatService.Disconnect();
        _view.CloseView();
    }

    void OnSettingsPressed()
    {
        _view.ShowError("Not implemented yet");
    }


    void OnMicrophoneTogglePressed()
    {
        if (_view.IsMicrophoneActive)
            _view.Mute();
        
        else
            _view.UnMute();

    }


    async Task OnDataFromMicrophoneReceived(byte[] rawBytesFromMic) 
    {
        byte[] encodedBytes = _audioCodec.Encode(rawBytesFromMic);
        await _voiceChatService.SendAsync(new VoiceChatMessage { RoomKey = _view.RoomKey , SenderUsername = _view.Username , VoiceData = encodedBytes });   
    }

    // service actions
    void OnDataRecieved(byte[] encodedVoiceBytes )
    {
        byte[] decodedBytes = _audioCodec.Decode(encodedVoiceBytes);
        
        _view.VoiceDataRecieved(decodedBytes);

    }
    





}
