FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy
MAINTAINER Kok How, Teh <funcoolgeek@gmail.com>
RUN apt update -y
RUN DEBIAN_FRONTEND=noninteractive apt install -y tzdata gnupg2 gnupg gnupg1
WORKDIR /app
RUN openssl genrsa -aes256 -passout pass:AspNetCoreWebApi -out server.key 4096
RUN openssl rsa -in server.key -out server.key.insecure -passin pass:AspNetCoreWebApi
RUN mv server.key server.key.secure
RUN mv server.key.insecure server.key
RUN touch /root/.rnd
RUN openssl req -new -newkey rsa:4096 -x509 -nodes -days 365 -keyout server.key -out server.crt -subj "/C=SG/ST=Singapore/L=Singapore /O=Kok How Pte. Ltd./OU=AspNetCoreWebApi/CN=localhost/emailAddress=funcoolgeek@gmail.com" -passin pass:AspNetCoreWebApi
RUN openssl pkcs12 -export -out /tmp/localhost.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:AspNetCoreWebApi
#dotnet dev-certs https --clean 
#dotnet dev-certs https -ep ./localhost.pfx -p AspNetCoreWebApi -v
#dotnet dev-certs https --trust 
ADD Web.Api/bin/Release/net6.0/publish/ .
EXPOSE 443
ENTRYPOINT ["dotnet", "Web.Api.dll"]
