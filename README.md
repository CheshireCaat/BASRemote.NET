# BASRemote.NET

[![NuGet version](https://badge.fury.io/nu/BASRemote.svg)](https://badge.fury.io/nu/BASRemote)
[![Build status](https://ci.appveyor.com/api/projects/status/se1coyoqblwm0imd?svg=true)](https://ci.appveyor.com/project/CheshireCaat/basremote-net)
[![GitHub issues](https://img.shields.io/github/issues/CheshireCaat/BASRemote.NET)](https://github.com/CheshireCaat/BASRemote.NET/issues)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**BASRemote** - .NET library, which allows you to **automate Google Chrome browser**.

In order to make it possible, BrowserAutomationStudio application is used. **BASRemote** allows you to call and control execution of functions created in BAS. Consider following example, you have a BAS function, which executes specified Google search query and returns result as a list of urls. Using this library, you can call that function in any C# application and obtain result. You can distribute applications written with **BASRemote** library as well.

# BrowserAutomationStudio

**BAS** - is application that allows you to automate any activities in Google Chrome browser with a help of visual programming and without knowing of any programming language. You can think of it as IDE created especially for browser automation:

![](https://bablosoft.com/landing2/screen-bas.png)

Check following link for more info:

[https://bablosoft.com/shop/BrowserAutomationStudio](https://bablosoft.com/shop/BrowserAutomationStudio)

# Installation

Install package via NuGet:

```
> Install-Package BASRemote
```

# Build from source

1. Clone this repo to your PC.
2. Compile source with VS2019 or Rider.
3. Add reference to _BASRemote.dll_ to your project.

# Quick start

Following code will search for _cats_ query in Google and output result into console. You can just copy paste this code and run it.

```csharp
//Set script name, and optionally auth details (login, password)
var ScriptOptions = new BASRemote.Options
{
	ScriptName = "TestRemoteControl" /* or "YourScriptName" */
};

//Create client
using (var ScriptClient = new BASRemote.BasRemoteClient(ScriptOptions))
{
	//Start application, this may take some time
	await ScriptClient.Start();

	//Set parameters for function
	var ScriptParams = new BASRemote.Objects.Params
	{
    		["Query"] = "cats"
	};

	//Run function and wait for result
	//Following function will return list of strings
	var Result = await ScriptClient.RunFunction(
		"GoogleSearch", /* or "YourFunctionName" */
		ScriptParams
	).GetTask<string[]>();

	//Iterate and output results
	foreach(var Link in Result)
	{
    		Console.WriteLine(Link);
	}

}
```

Checkout [wiki](https://github.com/CheshireCaat/BASRemote.NET/wiki) for more examples.

# Running custom code

Previous example used _TestRemoteControl_ project and _GoogleSearch_ function defined in it. In most cases you want to use your own projects and functions. In order to do it:

- Install BAS. Download using following [link](https://bablosoft.com/shop/BrowserAutomationStudio#download). **IMPORTANT** You need to be a premium user in order to create project with custom functions.
- Start [record mode](https://i.imgur.com/JrV7ua5.png) and create new function by using [function manager](https://i.imgur.com/yAjLu8v.png). BAS functions works like functions in any other languages. They can be called with parameters and can return value as a result. Functions help to incapsulate and reuse your code.
- Implement it. On following step you need to implement required functionality. Place code into the function that you have created on previous step. They will be called from C# code later. Function parameters will be sent from C# to BAS, while return value will be sent from BAS to C#. Working with BAS is out of scope of this article, check [BAS wiki](https://wiki.bablosoft.com/doku.php) for more info.
- Compile it and give it a name. Check this [article](https://wiki.bablosoft.com/doku.php?id=how_to_protect_your_script) more more instruction for compilation.
- Finally, **allow remote function execution** flag for script must be set. You can do that on following [page](https://bablosoft.com/bas/scripts). See [screenshot](https://i.imgur.com/BrkefIT.png) for more details.

After project with function is prepared, you can use it from C#. In order to do that, change script and function name in example above.

# How it works

Following diagram will explain project architecture:

![](https://i.imgur.com/9lfF3EJ.png)

**Running custom code** section explains how to prepare your project and upload it into the cloud. Portable BAS instance is downloaded and started automatically, it is also closed automatically when BASRemote gets disposed. Folder, where portable BAS instance is located by default is _data_ folder relative to executable. It can be customized by using `BASRemote.Options.WorkingDirectory` setting.

# Project example

You can use _TestRemoteControl_ project in order to test **BASRemote.NET** library. It is already uploaded into the cloud and can be used without authentication. List of available functions:

- `Add(X,Y)` - adds two numbers and return their sum.
- `SetProxy(Proxy,IsSocks5)` - sets proxy for current thread. _Proxy_ param is proxy string, _IsSocks5_ is string("true", "false") value indicates if proxy type is socks5. No return value.
- `CheckIp()` - returns remote IP of current thread. Uses ip.bablosoft.com service to test. Can be combined with _SetProxy_ function.
- `GoogleSearch(Query)` - performs Google query, returns result as a list of urls.

Project source code can be downloaded [here](https://drive.google.com/uc?id=1WQYzm-XaZhXUBWQYMM5T-sZ_tdcSfAwS&export=download)

# Dependencies

This library has dependencies on several excellent libraries:

[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

[WebSocketSharp](https://github.com/sta/websocket-sharp)

# License

**BASRemote.NET** has MIT license.

You can distribute applications using **BASRemote.NET** library, including commercial, to user, who don't have BAS premium subscription without any fees.

In order to create project with custom functions you need to have a BAS premium subscription.

In other words, only developers must have BAS premium subscription, not users.
