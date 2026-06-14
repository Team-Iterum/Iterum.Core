# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Iterum.Core is a monorepo of C# class libraries (.NET Standard 2.1, x64 only) that form the core framework for the Iterum game engine. Each library is independently versioned and published to NuGet.

## Build Commands

```bash
# Restore and build a specific project
dotnet restore Iterum.WebSockets/Iterum.WebSockets.csproj
dotnet build Iterum.WebSockets/Iterum.WebSockets.csproj

# Pack for NuGet (Release configuration)
dotnet pack Iterum.WebSockets/Iterum.WebSockets.csproj --configuration Release
```

There is no solution file — each project is built individually. There are no test projects in this repository.

## Library Dependency Graph

```
Iterum.Log           (standalone)
Iterum.Math          (standalone)
Iterum.Math2         (standalone, bundles Unity.Mathematics)
Iterum.Network       → Iterum.Math
Iterum.Core          → Iterum.Log, Iterum.Math
Iterum.Physics       → Iterum.Math
Iterum.PhysX         → Iterum.Physics, Iterum.Math
Iterum.ThingTypes    → Iterum.Physics, Iterum.Math, Iterum.Network
Iterum.Telepathy     → Iterum.Network, Iterum.Log
Iterum.WebSockets    → Iterum.Network, Iterum.Log
```

Inter-project NuGet references use wildcard versions (`Version="*"`), so the latest published version is always resolved.

## Architecture

**Networking layer:** `INetworkServer` (in Iterum.Network) is the core abstraction for network servers. It defines an event-driven API with `Received`, `Connecting`, `Connected`, `Disconnected` events and `Send`/`Disconnect`/`Stop` methods. Two implementations exist:
- `WebSocketsNetwork` (Iterum.WebSockets) — uses vtortola.WebSocketListener
- `TelepathyNetwork` (Iterum.Telepathy) — uses Telepathy TCP library

**Serialization contract:** Network packets implement `ISerializablePacketSegment` with `Serialize()`/`Deserialize()` operating on `ArraySegment<byte>`.

**Data types:** `NetworkMessage` carries received data + connection ID. `ConnectionData` carries connection ID + `IPEndPoint`.

## Code Conventions

- All libraries use `RootNamespace: Iterum` regardless of project name — types live in sub-namespaces like `Iterum.Network`, `Iterum.Logs`
- File-scoped namespaces (C# 10 style) in newer files; block-scoped in older files
- Logging uses `Iterum.Logs.Log` static class with a `const string LogGroup` per class
- Thread safety via `SemaphoreSlim` for serialized access, `Interlocked` for counters
- Async patterns use `ConfigureAwait(false)` and `CancellationToken` propagation
- CI runs on GitHub Actions (.github/workflows/dotnetcore.yml): builds all projects on push/PR to master, then packs and pushes to NuGet
