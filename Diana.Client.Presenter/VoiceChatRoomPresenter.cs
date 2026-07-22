using Diana.Core.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Diana.Core.Abstractions.Views;
using Diana.Core.Abstractions.Services;

namespace Diana.Client.Presenter;

public class VoiceChatRoomPresenter
{
    private readonly IVoiceChatRoomView _view;
    private readonly IVoiceChatService _voiceChatService;
    private readonly IChatService _chatService;
    private readonly IAudioCodec _audioCodec;

    public VoiceChatRoomPresenter(IVoiceChatRoomView view, IChatService chatService, IVoiceChatService voiceChatService, IAudioCodec audioCodec)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(voiceChatService);
        ArgumentNullException.ThrowIfNull(audioCodec);
        ArgumentNullException.ThrowIfNull(chatService);

        _view = view;
        _audioCodec = audioCodec;
        _voiceChatService = voiceChatService;
        _chatService = chatService;

        _voiceChatService.OnVoiceDataRecieved += OnDataRecieved;
        _chatService.OnMessageReceived += OnMessageReceived;

        _view.OnCopyRoomKeyPressed += OnCopyRoomKeyPressed;
        _view.OnLeaveRoomPressed += async () => await OnLeaveRoomPressed() ;
        _view.OnSettingsPressed += OnSettingsPressed;
        _view.OnMicrophoneTogglePressed += OnMicrophoneTogglePressed;
        _view.OnDataFromMicrophoneReceived += async (data) => await OnDataFromMicrophoneReceived(data);
        
        _view.IsMicrophoneActive = true;

        // Request the current member list when the view is first created
        RequestMemberList();
    }

    private async void RequestMemberList()
    {
        await _chatService.SendAsync(ClientRequest.GetVoiceChatRoomMembers(_view.RoomKey));
    }

    void OnCopyRoomKeyPressed()
    {
        _view.CopyRoomKeyToClipboard();
    }

    async Task OnLeaveRoomPressed()
    {
        await _chatService.SendAsync(ClientRequest.LeaveRoom(_view.RoomKey, _view.Username));
        _voiceChatService.Disconnect();
        
        _voiceChatService.OnVoiceDataRecieved -= OnDataRecieved;
        
        _chatService.OnMessageReceived -= OnMessageReceived;
        _view.CloseView();
    }

    void OnSettingsPressed()
    {
        _view.ShowError("Not implemented yet");
    }

    void OnMicrophoneTogglePressed()
    {
        if (_view.IsMicrophoneActive)
        {
            _view.Mute();
            _ = _chatService.SendAsync(ClientRequest.MicToggled(_view.RoomKey, _view.Username, true));
        }
        else
        {
            _view.UnMute();
            _ = _chatService.SendAsync(ClientRequest.MicToggled(_view.RoomKey, _view.Username, false));
        }
    }

    async Task OnDataFromMicrophoneReceived(byte[] rawBytesFromMic)
    {        
        byte[] encodedBytes = _audioCodec.Encode(rawBytesFromMic);
        await _voiceChatService.SendAsync(new VoiceChatMessage { RoomKey = _view.RoomKey, SenderUsername = _view.Username, VoiceData = encodedBytes });
    }

    void OnDataRecieved(byte[] encodedVoiceBytes)
    {
        byte[] decodedBytes = _audioCodec.Decode(encodedVoiceBytes);
        _view.VoiceDataRecieved(decodedBytes);
    }

    

    private void OnMessageReceived(ServerResponse msg)
    {
        switch (msg.Type)
        {
            case MessageType.RoomMemberList:
                HandleRoomMemberList(msg);
                break;

            case MessageType.MemberMicToggled:
            case MessageType.MemberSpeakingStatusChanged:
            case MessageType.UserJoined:
            case MessageType.UserLeft:
                RequestMemberList();
                break;
        }
    }

    private void HandleRoomMemberList(ServerResponse msg)
    {
        var members = JsonSerializer.Deserialize<List<VoiceChatRoomMember>>(msg.Content.ToString() ?? "[]")
                       ?? new List<VoiceChatRoomMember>();
        _view.SetMembers(members);
    }
}