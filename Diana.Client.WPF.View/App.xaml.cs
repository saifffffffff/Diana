using Diana.Client.Application;
using Diana.Client.Presenter;
using Diana.Client.WPF.View.Views;
using Diana.Core.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Diana.Client.WPF.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            
            var serviceCollection = new ServiceCollection();

            
            serviceCollection.AddSingleton<IChatService, ChatService>();
            serviceCollection.AddSingleton<IVoiceChatService, VoiceChatService>();
            serviceCollection.AddSingleton<IAudioCodec, OpusAudioCodec>();

            
            serviceCollection.AddTransient<LobbyView>();
            serviceCollection.AddTransient<CreateRoomView>();
            serviceCollection.AddTransient<ChatRoomView>();
            serviceCollection.AddTransient<VoiceChatRoomView>();
            serviceCollection.AddTransient<LobbyPresenter>();

            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            DIContainer.Initialize(serviceProvider);

            // Show main window
            var lobbyView = serviceProvider.GetRequiredService<LobbyView>();
            lobbyView.Show();
        }
    }

}
