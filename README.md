# BASRemote.NET

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build status](https://ci.appveyor.com/api/projects/status/se1coyoqblwm0imd?svg=true)](https://ci.appveyor.com/project/CheshireCaat/basremote-net)
[![NuGet version](https://badge.fury.io/nu/BASRemote.svg)](https://badge.fury.io/nu/BASRemote)

**BrowserAutomationStudioRemote (BASRemote)** - .NET library, which allows you to use projects created in BAS in your C# applications, work with ready-made functions and perform other actions.

# About BAS

**BAS (Browser Automation Studio)** - application that allows you to automate any actions in the browser. It uses the Chrome engine to work. You can create projects using the script editor and ready-made actions for work.

Some useful features:
* Ability to create fully autonomous applications.
* Ability to work with the database.
* Extending functionality with modules.
* Simple and powerful multithreading.
* 100% browser emulation.
* Built-in task scheduler.

Using this library, you can work with your BAS projects via C#. This opens up endless possibilities for automation and the creation of complex full-fledged applications.

You can find out more information using these links:

[Project site](https://bablosoft.com/shop/BrowserAutomationStudio#)

[YouTube](https://www.youtube.com/channel/UC_fHAkJk4dNj8gnFbt55tHg)

[Forum](https://community.bablosoft.com/)

[Wiki](http://wiki.bablosoft.com/)

On project site you can find download links too.

# How it works

Functions in BAS can act as separate sub-projects. Imagine your script is a class. BAS functions are methods in your class. You can run them in separate threads, you can add parameters and specify the return value. 

This means that in the functions you can perform any actions - manage separate browser instances, save data using the database and other resources, work with HTTP client etc.

In order to add a function to your project, follow these steps:
* Click on the footer in the script editor (a small white panel with the text **Main**).
* Click on the button with a plus sign.
* Specify a function name.
* Specify function parameters (if necessary).
* Specify the return value (if necessary).
* Clik on the **Save Changes** button.

After that, you can describe the logic you need using action blocks.

Here is an example function **GoogleSearch** which collects links from a Google Search using the specified query and returns the result as an array:

[Function (Picture)](https://imgur.com/9VuhEN9)

This library can use the functions created in your project and execute them in separate BAS threads. You can control the lifetime of threads and call several functions in them, or use simplified calls using client methods.

# Quick example

The code shown in the example below performs the **Add** function, which adds two numbers and displays the result in the console:

```csharp
using System.Threading.Tasks;
using BASRemote;
using BASRemote.Objects;

namespace BASRemoteExample
{
    internal static class Program
    {
        private static async Task Main()
        {
            // Specify script options
            var options = new Options
            {
                ScriptName = "RemoteControlTest"
            };
            
            using (var client = new BasRemoteClient(options))
            {
                // Start the client
                await client.Start();

                // Run Add function and wait for result
                var result = await client.RunFunction<int>("Add", new Params
                {
                    ["X"] = 15,
                    ["Y"] = 25
                });
                
                // Print result
                System.Console.WriteLine($"15 + 25 = {result}");
                System.Console.ReadKey();
            }
        }
    }
}
```

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
