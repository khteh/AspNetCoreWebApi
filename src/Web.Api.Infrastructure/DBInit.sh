#!/bin/bash
rm -rf migrations
dotnet ef -s ../Web.Api.Infrastructure migrations add appidentitydbcontext.initial --context AppIdentityDbContext
dotnet ef -s ../Web.Api.Infrastructure migrations add appdbcontext.initial --context AppDbContext
dotnet ef -s ../Web.Api.Infrastructure database update --context AppIdentityDbContext
dotnet ef -s ../Web.Api.Infrastructure database update --context AppDbContext
