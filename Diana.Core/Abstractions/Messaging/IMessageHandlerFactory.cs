using Diana.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Core.Abstractions.Messaging;

public interface IMessageHandlerFactory
{
    public IMessageHandler Create(MessageType messageType);

}
