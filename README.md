# Diana - Real-time Chat & Voice Application

Diana is a real-time communication application built with .NET that supports both text chat and voice communication. It follows a client-server architecture with a WPF desktop client.

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        Diana Solution                            │
├──────────────┬──────────────┬──────────────┬────────────────────┤
│ Diana.Server │ Diana.Core   │ Diana.Client │ Diana.Client.WPF   │
│              │              │ .Application │     .View          │
├──────────────┼──────────────┼──────────────┼────────────────────┤
│ • TCP Server │ • Abstractions│ • ChatService│ • WPF Views       │
│ • UDP Voice  │ • DTOs       │ • VoiceChat  │ • Presenters      │
│   Relay      │ • Enums      │   Service    │ • Controls        │
│ • Room Mgmt  │              │ • Opus Codec │ • DI Container    │
│ • Session Mgmt│             │              │                   │
└──────────────┴──────────────┴──────────────┴────────────────────┘
```

## 🚀 Features

- **Text Chat**: Real-time messaging in chat rooms
- **Voice Chat**: Low-latency voice communication using UDP
- **Room Management**: Create and join chat/voice rooms
- **Session Management**: User sessions with connection tracking
- **Modern UI**: WPF-based desktop client with MVVM pattern
- **Audio Encoding**: Opus codec for efficient voice transmission

## 📁 Project Structure

### Diana.Server
The server component handling all backend operations:
- **Networking**: TCP connection acceptor, TCP network channels, UDP voice stream relay
- **Rooms**: Room management (chat & voice), chat room implementation
- **Sessions**: Session management for connected clients
- **Messaging**: Message handlers for all client requests (join, leave, send message, mic toggle, speaking status, etc.)

### Diana.Core
Shared library containing:
- **Abstractions**: Interfaces for services, networking, rooms, sessions, serialization
- **DTOs**: Data transfer objects for client-server communication
- **Enums**: RoomType (Chat, Voice)

### Diana.Client.Application
Client-side application logic:
- **ChatService**: TCP-based chat communication
- **VoiceChatService**: UDP-based voice communication
- **OpusAudioCodec**: Audio encoding/decoding using Opus

### Diana.Client.Presenter
MVVM Presenters (ViewModels):
- `LobbyPresenter` - Main lobby view logic
- `ChatRoomPresenter` - Chat room view logic
- `VoiceChatRoomPresenter` - Voice chat room view logic
- `CreateRoomPresenter` - Room creation view logic

### Diana.Client.WPF.View
WPF UI Layer:
- **Views**: LobbyView, CreateRoomView, ChatRoomView, VoiceChatRoomView
- **Controls**: VoiceRoomMemberTile, FriendMessageControl, CircularPictureBox
- **Styles**: Theme.xaml for consistent styling
- **DI**: Microsoft.Extensions.DependencyInjection for dependency injection

## ⚙️ Configuration

Server configuration (`Diana.Server/appconfig.json`):
```json
{
  "server-ip": "127.0.0.1",
  "tcp-port": 5000,
  "udp-port": 5001
}
```

## 🛠️ Getting Started

### Prerequisites
- .NET 9.0 SDK or later
- Windows (for WPF client)

### Building the Solution
```bash
dotnet build Diana.slnx
```

### Running the Server
```bash
cd Diana.Server
dotnet run
```

### Running the Client
```bash
cd Diana.Client.WPF.View
dotnet run
```

## 🔧 Technology Stack

- **Runtime**: .NET 9.0
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel) with Presenters
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Serialization**: System.Text.Json
- **Audio Codec**: Opus (via OpusAudioCodec)
- **Networking**: TCP (chat), UDP (voice)

## 📡 Communication Protocol

The application uses JSON over TCP for chat messaging and raw UDP for voice streaming.

### Message Types (Client → Server)
- `CreateRoomRequest` - Create a new chat/voice room
- `JoinRoomRequest` - Join an existing room
- `LeaveRoomRequest` - Leave current room
- `SendMessageRequest` - Send a chat message
- `SetUdpEndPointRequest` - Register UDP endpoint for voice
- `GetRoomListRequest` - Get list of available rooms
- `GetVoiceChatRoomMembersRequest` - Get voice room members
- `MicToggledRequest` - Toggle microphone on/off
- `SpeakingStatusChangedRequest` - Update speaking status

### Message Types (Server → Client)
- `ServerResponse` - Generic response with success/error
- `RoomCreatedNotification` - Room was created
- `UserJoinedNotification` - User joined the room
- `UserLeftNotification` - User left the room
- `MessageReceivedNotification` - New chat message
- `VoiceChatMemberListNotification` - Voice room member list
- `UserMicToggledNotification` - User mic status changed
- `UserSpeakingStatusChangedNotification` - User speaking status changed

## 🎯 Key Design Patterns

- **Dependency Injection**: All components use DI for loose coupling
- **Factory Pattern**: MessageHandlerFactory creates appropriate handlers
- **Repository Pattern**: RoomManager manages room storage
- **Observer Pattern**: Events for message received, user joined/left
- **MVVM**: Separation of UI (Views) from logic (Presenters)

## 📝 License

This project is for educational/demonstration purposes.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request