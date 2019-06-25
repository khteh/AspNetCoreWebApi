#!/bin/bash
#$(aws ecr get-login --no-include-email)
docker build -t khteh/asp.netcoreapistarter .
docker push khteh/asp.netcoreapistarter:latest
