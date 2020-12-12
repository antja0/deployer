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

### API / Webhook

Webhook listens to POST deployer/api/{eventId}

Different events can be configured, default are "push", "pull-request" and "release".
So for example if you want to deploy to dev servers on one event and to production with another.

When deployer receives webhook request for the first time new application is created.

After this build script must be configured at the deployer server manually (for extra security).

On second time and onwards a new folder is created at the deployer server: `deployer/{applicationid}/{buildId}`
Build script is run at the folder. Something like this:
```sh
git clone 
dotnet build
```
Make sure all dependencies etc. required in building the application are installed on deployer server.
Then output in specified output folder is zipped and stored at the deployer server for future deployments.

### UI

Deployer.UI can basically be thought of as an admin panel that triggers deployments.
It can also be used to organize projects into deployment groups etc.
There is also an option to configure deployments (or some of them) to be automatic.
It cannot be used to modify build scripts, it can only eg. edit which build script is used for which application.

## Nodes

Default setup is that each node contains multiple projects - which in turn contain multiple applications. Each server has one node that receives built files from Deployer.

The Node included is designed for IIS deployments, but (if you want to code your own) essentially Node does these things:
- Upon initial setup the node registers/adds itself to the deployer. (POST deployer/api/nodes)
- Listens to (POST node/api/deploy) to trigger deployment
- When deployment is triggered Node downloads the specified version from deployer (GET deployer/api/Versions)

## Security

All communication between Deployer components is secured with HTTPS and [HMAC](https://en.wikipedia.org/wiki/HMAC).

Deployer and nodes can also be IP restricted on software level. TODO: instructions on how to.

Preferrably Deployer networking should be setup so that:
- Deployer.API receives requests from anywhere (eg. Github or node registrations).
- Nodes allowing connections only from deplyer.
- Deployer.UI in intranet, allowing no outside connections (basically for triggering deployments).

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

## Usage

Webhook (in Deployer.Api) listens to POST events in this route:

`/api/{eventId}`

If using Github you can call these webhooks:

- `/api/push` push event
- `/api/release` release event
- `/api/pull-request` pull request event 

Deployer fetched calling application information from webhook [payload](https://docs.github.com/en/free-pro-team@latest/developers/webhooks-and-events/webhook-events-and-payloads).
