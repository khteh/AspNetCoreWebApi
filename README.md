# AspNetCoreWebApi

An ASP.NET 7.0 Web API, SignalR and GRPC project to quickly bootstrap new projects. Includes Identity, JWT authentication w/ refresh tokens. The design of the application is driven by both Domain-Driven-Design and Clean Architecture (https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). It makes use of Command and Decorator pattern, and MediatR for in-process synchronous messaging.

# Database Setup

- Uses MySQL.
- Install/update dotnet ef tool:

```
$ dotnet tool install --global dotnet-ef
$ dotnet tool update --global dotnet-ef
```

- Either run the application / `Web.Api.Infrastructure` project and the DB will be automatically created or:
- Apply database migrations to create the db. From a command line within the `Web.Api.Infrastructure` project folder use the dotnet CLI to run :

```
$ cd src/Web.Api.Infrastructure
$ dotnet ef database update -c AppDbContext
$ dotnet ef database update -c AppIdentityDbContext
```

# SignalR Client Setup

- `npm install @microsoft/signalr`
- Copy content of `node_modules\@microsoft\signalr\dist\browser` to `wwwroot/lib/signalr`

# Visual Studio

Open the solution file <code>AspNetCoreWebApi.sln</code> and build/run.

# Visual Studio Code

- `Ctrl`+`Shift`+`B` to build
- `F5` to start debug session

## Unit Testing

- Install .Net Core Test Explorer
- `echo fs.inotify.max_user_instances=524288 | sudo tee -a /etc/sysctl.conf && sudo sysctl -p`
  - https://github.com/dotnet/aspnetcore/issues/8449

# Swagger Enabled

To explore and test the available APIs simply run the project and use the Swagger UI @ http://localhost:{port}/swagger/index.html

The available APIs include:

- POST `/api/accounts` - Creates a new user.
- POST `/api/auth/login` - Authenticates a user.
- POST `/api/auth/refreshtoken` - Refreshes expired access tokens.
- POST `/api/accounts/changepassword` - Change password.
- POST `/api/accounts/resetpassword` - Reset password.
- GET `/api/accounts/lock/{id}` - Locks a user account id.
- GET `/api/accounts/unlock/{id}` - Unlocks a user account id.
- DELETE `/api/accounts/{username}` - Delete a user using username.
- GET `/api/protected` - Protected controller for testing role-based authorization.

# Continuous Integration:

- Integrated with CircleCI

# Kubernetes

- If ingress uses a prefix path, the prefix needs to be added as an environment variable `PATH_BASE` (or `appsettings.json` mounted from ConfigMap)
- Swagger does NOT work when the `PATH_BASE` is not `/` due to an issued filed as https://github.com/dotnet/aspnetcore/issues/42559
