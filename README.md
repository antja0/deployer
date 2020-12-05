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

Communication between deployer and nodes (as well as webhook requests) are secured with [HMAC](https://en.wikipedia.org/wiki/HMAC).
However I'd recommend you to host deployer and nodes in the same network so that these endpoints are not open to the internet or something, because I don't really trust my skills with this :smile:
**I do not take any responsibility, please do your own research before using.**

## Development

After updating models, run [ef migrations](https://docs.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli) with powershell from repository root:
```ps
dotnet ef migrations add InitialCreate --project Deployer.Data
```
