# OIDC.Core 

<img src="logo_transparent.png" style="max-width: 300px; height: auto" />

Toy OAuth2.0 & OpenID Connect identity provider built on C# & Dotnet ~~5~~ ~~6~~ ~~7~~ 8 (project's been going a while)
built primarily as a way of understanding the OAuth and OpenID Connect protocols.

### Developer Setup
- Ensure dotnet & asp.net SDKs are installed - see [documentation](https://learn.microsoft.com/en-us/dotnet/core/install/) for instructions
- Ensure docker/podman/equivalent is installed - see [docker documentation](https://www.docker.com/) for details


- Clone repo
```bash
git@github.com:DLMousey/OIDC.Core-Redux.git
```
- Install dependencies
```bash
cd OIDC.Core-Redux \
dotnet --project OIDC.Core-Redux restore
```
- Start supporting services
```bash
docker compose up
```
- Start API server
```bash
dotnet --project OIDC.Core-Redux start
```

Tests can be run with `dotnet test`
```
dotnet test
```
