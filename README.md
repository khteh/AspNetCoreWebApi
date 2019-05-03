# AspNetCoreApiStarter
An ASP.NET Core (v2.2) Web API project to quickly bootstrap new projects.  Includes Identity, JWT authentication w/ refresh tokens.

# Database Setup
- Uses MySQL.
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
- DELETE `/api/accounts/{username}` - Delete a user using username.
- GET `/api/protected` - Protected controller for testing role-based authorization.

# Continuous Integration:
* Integrated with CircleCI

# Contact
mark@fullstackmark.com
funcoolgeek@gmail.com
