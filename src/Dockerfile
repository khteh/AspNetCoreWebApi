FROM mcr.microsoft.com/dotnet/aspnet:10.0
LABEL org.opencontainers.image.authors="Kok How, Teh <funcoolgeeek@gmail.com>"
RUN apt update -y
RUN DEBIAN_FRONTEND=noninteractive apt install -y tzdata gnupg2 gnupg gnupg1
WORKDIR /app
ADD robots.txt .
RUN touch /root/.rnd
RUN openssl req -new -newkey rsa:4096 -x509 -nodes -days 365 -keyout server.key -out server.crt -subj "/C=SG/ST=Singapore/L=Singapore /O=Kok How Pte. Ltd./OU=AspNetCoreWebApi/CN=localhost/emailAddress=funcoolgeek@gmail.com" -passin pass:AspNetCoreWebApi -addext "subjectAltName = DNS:localhost"
RUN openssl pkcs12 -export -out /tmp/localhost.pfx -inkey server.key -in server.crt -passout pass:AspNetCoreWebApi
ADD Web.Api/bin/Release/net10.0/publish/ .
EXPOSE 443
#ENTRYPOINT ["dotnet", "Web.Api.dll"]
ENTRYPOINT ["./Web.Api"]
