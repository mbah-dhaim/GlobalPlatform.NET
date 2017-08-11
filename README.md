# GlobalPlatform.NET [![Build status](https://ci.appveyor.com/api/projects/status/h0sci56qicwehbq1?svg=true)](https://ci.appveyor.com/project/jamesharling/globalplatform-net)
A fluent .NET API to manage GlobalPlatform smartcards. The library is currently built against version 2.3 of the GlobalPlatform specification ([link](https://www.globalplatform.org/specificationscard.asp)), and includes support for every command in the specification, plus support for Secure Channel Protocol 02 (SCP02).

## Getting started
Grab the package from NuGet, which will install all dependencies.

`Install-Package GlobalPlatform.NET -Pre`

## Usage
GlobalPlatform.NET is a fluent .NET API for GlobalPlatform smartcards. The API lets you programatically generate APDU commands, interrogate card contents and work with Secure Channels, where the API will manage security such as encryption and MACing.

Every command can be built by calling the `Build` method on the command object; this will return a fluent builder which will then build the command to your specification:

```csharp
SelectCommand.Build
  .SelectFirstOrOnlyOccurrence()
  .Of(aid)
  
-> 00-A4-04-00-08-FF-FF-FF-FF-FF-FF-FF-FF-00
                
DeleteCommand.Build
  .DeleteCardContent()
  .WithAID(aid)
  .AndRelatedObjects()
  .UsingToken(token)

-> 80-E4-00-80-14-4F-08-FF-FF-FF-FF-FF-FF-FF-FF-9E-08-EE-EE-EE-EE-EE-EE-EE-EE-00
  
SetStatusCommand.Build
  .SetIssuerSecurityDomainStatus()
  .To(CardLifeCycleCoding.Initialized)

-> 80-F0-80-07
```

## Secure Channels
Coming soon.
