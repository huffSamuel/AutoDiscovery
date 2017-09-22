# AutoDiscovery

AutoDiscovery is a .NET library for client/server discovery via UDP.

[![Nuget](https://img.shields.io/nuget/v/AoBD.AutoDiscovery.svg)](https://www.nuget.org/packages/AoBD.AutoDiscovery/)

# Installation

` PM> Install-Package AoBD.AutoDiscovery ` 

# Use

Create a server:
```csharp
IServer server = new Server(16000);
server.ClientConnected += OnClientConnected;
server.Start();
// Go about your work here
server.Stop();
```

Create a client:
```csharp
int pollingTime = 2000;
IClient client = new Client(16000, pollingTime);
client.ServerDiscovered += OnServerDiscovered;
client.Start();
// Go about your work here
client.Stop();
```

The server will discover clients that are broadcasting to the network and notify the consumer via the `ClientConnected` event; likewise the client will notify via the `ServerDiscovered` event.

**Communication is not managed at this point and clients will broadcast and servers will listen until `Stop()` is called. If not managed correctly this will lead to duplicate server/client discovery.**

# Requirements
.NET Framework 4.5

# Support
Please submit issues via the issue tracker.

# Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

# License
[MIT](https://github.com/huffSamuel/AutoDiscovery/blob/master/LICENSE)