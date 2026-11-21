#!/bin/bash
# The JWT signing key is never baked into the image or committed to the repository.
# Generate one (64+ random bytes) and keep it in a secret store, e.g.: export AuthSettings__SecretKey="$(openssl rand -base64 64 | tr -d '\n')"
: "${AuthSettings__SecretKey:?Set AuthSettings__SecretKey to a random key of at least 64 bytes, e.g. openssl rand -base64 64 | tr -d '\n'}"
docker pull khteh/asp.netcorewebapi:latest
docker run -d -p5000:5000 -e AuthSettings__SecretKey khteh/asp.netcorewebapi:latest
