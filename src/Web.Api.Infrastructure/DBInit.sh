#!/bin/bash
dotnet ef -s ../Web.Api.Infrastructure migrations add initial --context AppIdentityDbContext
dotnet ef -s ../Web.Api.Infrastructure migrations add initial --context AppDbContext
dotnet ef -s ../Web.Api.Infrastructure database update --context AppIdentityDbContext
dotnet ef -s ../Web.Api.Infrastructure database update --context AppDbContext
