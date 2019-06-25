FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.5-bionic
MAINTAINER Kok How, Teh <funcoolgeek@gmail.com>
WORKDIR /app
ADD Web.Api/bin/Release/netcoreapp2.2/publish/ .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Web.Api.dll"]
