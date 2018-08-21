# GlobalPlatform.NET [![Build status](https://ci.appveyor.com/api/projects/status/h0sci56qicwehbq1?svg=true)](https://ci.appveyor.com/project/jamesharling/globalplatform-net)
A fluent .NET API to manage GlobalPlatform smartcards. The library is currently built against version 2.3 of the GlobalPlatform specification ([link](https://www.globalplatform.org/specificationscard.asp)), and includes support for every command in the specification, plus support for Secure Channel Protocol 02 (SCP02).

## Getting started
Grab the package from NuGet, which will install all dependencies.

`Install-Package GlobalPlatform.NET`

## Usage
GlobalPlatform.NET is a fluent .NET API for GlobalPlatform smartcards. The API lets you programatically generate APDU commands, interrogate card contents and work with Secure Channels, where the API will manage security such as encryption and MACing.

Every command can be built by calling the `Build` method on the command object; this will return a fluent builder which will then build the command to your specification:

```csharp
SelectCommand.Build
  .SelectFirstOrOnlyOccurrence()
  .Of(aid)
  
// -> 00-A4-04-00-08-FF-FF-FF-FF-FF-FF-FF-FF-00
                
DeleteCommand.Build
  .DeleteCardContent()
  .WithAID(aid)
  .AndRelatedObjects()
  .UsingToken(token)

// -> 80-E4-00-80-14-4F-08-FF-FF-FF-FF-FF-FF-FF-FF-9E-08-EE-EE-EE-EE-EE-EE-EE-EE-00
  
SetStatusCommand.Build
  .SetIssuerSecurityDomainStatus()
  .To(CardLifeCycleCoding.Initialized)

// -> 80-F0-80-07
```

## Secure Channels
GlobalPlatform.NET currently supports secure communication using SCP02 i=15.

To begin a secure channel session, first issue an INITIALIZE UPDATE command to a card and record the response:

```csharp
InitializeUpdateCommand.Build
    .WithKeyVersion(keyVersion)
    .WithHostChallenge(out byte[] hostChallenge)
    .AsApdu();
```

A secure channel session can then be created:

```csharp
SecureChannel.Setup
    .Scp02()
    .Option15()
    .ChangeSecurityLevelTo(SecurityLevel.CMac)
```

Then specify your static card keys:

```csharp
    // Individual keys
    .UsingEncryptionKey(key1)
    .AndMacKey(key2)
    .AndDataEncryptionKey(key3)

    // Keys defined in a single object
    .UsingKeysFrom(keys, k => k.Key1, k => k.Key2, k => k.Key3)
```

And the parameters from the `INITIALIZE UPDATE` command:

```csharp
    .WithHostChallenge(hostChallenge)
    .AndInitializeUpdateResponse(initializeUpdateResponse)
```

Lastly, establish the session:
```csharp
    // Exceptions will be raised if there are authentication issues
    .Establish();

    // Exceptions will be suppressed
    .TryEstablish(out var secureChannelSession);
```

The secure channel session will verify that the correct keys have been used and that mutual authentication has taken place successfully. Afterwards, the session will be populated with the current session keys and MACs, and will be available for use to secure further communication with the card:

```csharp
var apdu = GetStatusCommand.Build
    .GetStatusOf(GetStatusScope.IssuerSecurityDomain)
    .AsApdu();
    
// -> 80-F2-80-00-02-4F-00-00

secureChannelSession.SecureApdu(apdu);

// -> 84-F2-80-00-0A-4F-00-C8-80-21-A2-17-85-DA-7A-00
```

Session keys can be provided instead of static keys, should you require this.
