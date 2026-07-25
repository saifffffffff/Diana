using Diana.Core.Abstractions.Networking;
using Diana.Core.Abstractions.Sessions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Diana.Server.Networking;

class TcpConnectionAcceptor(Func<TcpClient, INetworkChannel> networkChannelFactory, Func<INetworkChannel, ISession> sessionFactory) : IConnectionAcceptor
{
    public async Task RunAsync()
    {
        // recieve connection -> create a client handler 
        var listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();

        while (true)
        {
            var tcp = await listener.AcceptTcpClientAsync();


            // create a TcpService instance bound to the accepted TcpClient
            INetworkChannel networkChannel = networkChannelFactory(tcp);

            // create the client session handler; serializer will be resolved from the service provider
            //var client = ActivatorUtilities.CreateInstance<ISession>(serviceProvider , networkService);
            ISession session = sessionFactory(networkChannel);

            Console.WriteLine("Client connected.");

            Task.Run(async () => await session.RecieveAsync());

        }
    }
}
