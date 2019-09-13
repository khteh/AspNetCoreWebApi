#!/bin/bash
rm -f server.csr server.crt server.key server.key.insecure localhost.pfx
#openssl genrsa -des3 -passout pass:P@$$w0rd -out server.key 4096
openssl genrsa -aes256 -passout pass:4xLabs.com -out server.key 4096
openssl rsa -in server.key -out server.key.insecure -passin pass:4xLabs.com
mv server.key server.key.secure
mv server.key.insecure server.key
openssl req -new -newkey rsa:4096 -x509 -nodes -days 3650 -keyout server.key -out server.crt -subj "/C=SG/ST=Singapore/L=Singapore /O=4xLabs Pte. Ltd./OU=Biz4x/CN=localhost/emailAddress=info@4xlabs.com" -passin pass:4xLabs.com
#openssl pkcs12 -export -out localhost.pfx -inkey server.key -in server.crt -passin pass:4xLabs.com -passout pass:4xLabs.com
openssl pkcs12 -export -out localhost.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:4xLabs.com
#dotnet dev-certs https --clean
#dotnet dev-certs https -ep ./localhost.pfx -p 4xLabs.com -v
#dotnet dev-certs https --trust
cp localhost.pfx /tmp
