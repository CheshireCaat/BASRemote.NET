# BASRemote.NET

**BrowserAutomationStudioRemote (BASRemote)** - .NET library that allows you to remotely manage private scripts created using BAS.
This uses a portable version of the engine to execute the script (FastExecuteScript). Interaction with the script is based on the same principles as the web interface (via the WebSocket connection).

# How to install

Install via NuGet:

```
	> Install-Package BASRemote
```

Build from source:

1. Clone this repo to your PC.
2. Compile source with VS2019 or Rider.
3. Add reference to ```BASRemote.dll``` to your project.

# Getting started

Checkout [wiki](https://github.com/CheshireCaat/BASRemote.NET/wiki)

# Dependencies
This library has dependencies on several excellent libraries:

[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

[WebSocketSharp](https://github.com/sta/websocket-sharp)

# License
This project is licensed under the MIT license.