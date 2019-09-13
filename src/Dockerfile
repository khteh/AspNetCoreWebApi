FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.6-bionic
MAINTAINER Kok How, Teh <funcoolgeek@gmail.com>
RUN apt update -y
RUN DEBIAN_FRONTEND=noninteractive apt install -y tzdata
WORKDIR /app
RUN openssl genrsa -aes256 -passout pass:4xLabs.com -out server.key 4096
RUN openssl rsa -in server.key -out server.key.insecure -passin pass:4xLabs.com
RUN mv server.key server.key.secure
RUN mv server.key.insecure server.key
RUN touch /root/.rnd
RUN openssl req -new -newkey rsa:4096 -x509 -nodes -days 3650 -keyout server.key -out server.crt -subj "/C=SG/ST=Singapore/L=Singapore /O=4xLabs Pte. Ltd./OU=Biz4x/CN=localhost/emailAddress=info@4xlabs.com" -passin pass:4xLabs.com
RUN openssl pkcs12 -export -out /tmp/localhost.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:4xLabs.com
#dotnet dev-certs https --clean 
#dotnet dev-certs https -ep ./localhost.pfx -p 4xLabs.com -v
#dotnet dev-certs https --trust 
ADD Web.Api/bin/Release/netcoreapp2.2/publish/ .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Web.Api.dll"]
