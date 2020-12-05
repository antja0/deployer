# Deployer [WIP]

Deployer is for building continuous deployment pipeline from eg. github.

Whole Deployer project consists of Deployer (this repository) and nodes.

![diagram](docs/images/diagram.png)

## Deployer

- Keeps track of different applications, nodes and versions.
- Clones and builds the applications and then sends them securely to the desired nodes for deployment.
- Has webhook that is Github compatible. (You can set webhooks at github.com/user/repository/settings/hooks).
Webhook requests register new applications (if not yet present) and versions to the deployer.
- Can be configured to accept only repositories of certain organization

## Nodes

- Are setup on servers which contain multiple projects - which in turn contain multiple applications. Each server has one node that receives built files from Deployer.
- Upon initial setup the node registers itself to the deployer.
- When deployment is triggered new releases are uploaded to the registered nodes. Nodes will move downloaded files into their respective project and application folders under configured project root folder.
If not yet present, those folders will be created.

## Security

All communication between Deployer components is secured with HTTPS and [HMAC](https://en.wikipedia.org/wiki/HMAC).

Preferrably Deployer networking should be setup so that:
- Deployer.API receives requests from anywhere (eg. Github or node registrations).
- Deployer.Node:s allowing connections only from Deployer.UI.
- Deployer.UI in intranet, allowing no outside connections.

**I do not take any responsibility, please do your own research before using.**

## Development

Tools:
- Visual Studio
- ReSharper
- Docker (optional, can also just run from vs)
- Database server eg. SQL Server

Configure (appsettings.Development.json) Deployer.UI and Deployer.API to use same database.
Configure database to allow TCP/IP connections to port 1337 (default).

After updating models, run [ef migrations](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli) with powershell from repository root:
```ps
dotnet ef migrations add InitialCreate --project Deployer.Data
```
