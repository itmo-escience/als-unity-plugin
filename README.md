# ALS Unity Plugin

Plugin for integrate Cifrasoft Offline Fingerprint ACR from [Cifrasoft](http://www.cifrasoft.com/) into Unity project. Support iOS and Android projects. Cifrasoft Offline Fingerprint ACR is an acoustic fingerprinting solution which enables automatic content recognition directly on mobile device without server interaction.
Cifrasoft Offline Fingerprint ACR is suitable for Second Screen applications, which require synchronization between a piece of content and mobile application.

## Setup
For setup plugin you will need to

- Import `ALSUnityPlugin.unitypackage`([download](https://github.com/itmo-escience/als-unity-plugin/releases/download/1.0.0/ALSUnityPlugin.unitypackage)) into your Unity project.
- Sign up at Offline Fingerprint Database Management System via Web-panel for get your user name and manage database.
- Copy your user name into TestALSPlugin script "UserName" field on "ALSPluginObject" game object in ALSPluginExampleScene scene.

## Build 
For Android build you no need do extra steps, just build your project. For iOS build you will need to
- Go iOS Player Settings and define Microphone Usage Description.

## Usage
At moment plugin allow sync your application by sound recorded from microphone. See example demo scene for more information.

## C# API
`ALSPlugin` is static class for work with Cifrasoft Offline Fingerprint ACR.

### Methods
Method for initialize SDK with given user name.
```c#
void ALSPlugin.InitSDK(string userName)
```
Mrthod for start searching (continuous recognition) thread with recoding queue. Use it for 'continuous recognition' mode.
```c#
void ALSPlugin.StartSearch()
```
Method for stop searching (continuous recognition) thread.
```c#
void ALSPlugin.StopSearch()
```
Method for perform one-time search.
```c#
void ALSPlugin.SingleSearch()
``` 

### Properties
Current AudioManager state property.
```c#
ALSAudioManagerState AudioManagerState
```
Current SearchManager state property.
```c#
ALSSearchManagerState SearchManagerState
```
Last recognited track ID property. This ID taken from Database Management System. If not recognized this value equal -1.
```c#
int LastTrackId
``` 
Last recognited track offset in seconds. If not recognized this value equal -1.
```c#
int LastTrackOffset
``` 

### Delegates
Action delegate for AudioManager state change handling.
Arguments:
- Current AudioManager state
- Error message. (empty if not errored).
```c#
Action<ALSAudioManagerState, string> OnStateChangeEvent
```

Action delegate for SearchManager state change handling. 
Arguments:
- Current SearchManager state
- Error message (empty if not errored)
- Recognized track ID (-1 if not recognized)
- Recognized track offset (-1 if not recognized)
```c#
Action<ALSSearchManagerState, string, int, int> OnSearchStateChangeEvent;
```

----
