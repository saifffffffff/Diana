using Diana.Core.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Diana.Client.WPF.View
{
    /// <summary>
    /// Static helper class to access DI container from anywhere in the WPF application.
    /// Initialized in App.xaml.cs OnStartup.
    /// </summary>
    public static class DIContainer
    {
        private static IServiceProvider? _serviceProvider;
        
        public static void Initialize(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Get a service from the DI container.
        /// </summary>
        public static T GetService<T>() where T : notnull
        {
            
            if (_serviceProvider == null)
                throw new InvalidOperationException("DIContainer not initialized. Call DIContainer.Initialize in App.OnStartup.");

            return _serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Convenience properties for commonly used services
        /// </summary>
        public static IChatService ChatService => GetService<IChatService>();
        public static IVoiceChatService VoiceChatService => GetService<IVoiceChatService>();
        public static IAudioCodec AudioCodec => GetService<IAudioCodec>();

        
    }
}
