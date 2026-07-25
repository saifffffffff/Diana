using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Core.Abstractions.Networking;

public interface IConnectionAcceptor
{
    Task RunAsync();
}
