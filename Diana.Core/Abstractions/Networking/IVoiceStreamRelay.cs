using System;
using System.Collections.Generic;
using System.Text;

namespace Diana.Core.Abstractions.Networking;

public interface IVoiceStreamRelay
{
    Task RunAsync();
}
