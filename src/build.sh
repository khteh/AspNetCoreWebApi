#!/bin/bash
#$(aws ecr get-login --no-include-email)
docker build -t khteh/asp.netcorewebapi .
docker push khteh/asp.netcorewebapi:latest
