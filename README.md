# AspNetCoreApiStarter
An ASP.NET 6.0 Web API, SignalR and GRPC project to quickly bootstrap new projects. Includes Identity, JWT authentication w/ refresh tokens. The design of the application is driven by both Domain-Driven-Design and Clean Architecture (https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html). It makes use of Command and Decorator pattern, and MediatR for in-process synchronous messaging.

# Database Setup
- Uses MySQL.
- Install dotnet ef tool: <code>**dotnet tool install --global dotnet-ef**</code>
- Apply database migrations to create the db.  From a command line within the *Web.Api.Infrastructure* project folder use the dotnet CLI to run : 
- <code>Web.Api.Infrastructure>**dotnet ef database update --context AppDbContext**</code>
- <code>Web.Api.Infrastructure>**dotnet ef database update --context AppIdentityDbContext**</code>

# Visual Studio
Open the solution file <code>AspNetCoreApiStarter.sln</code> and build/run.

# Visual Studio Code
Open the <code>src</code> folder and <code>F5</code> to build/run.

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
* Integrated with CircleCI
