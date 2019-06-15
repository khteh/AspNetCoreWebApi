#!/bin/bash
#$(aws ecr get-login --no-include-email)
docker build -t asp.netcoreapistarter .
#docker tag redis-cluster-init:latest 701969852130.dkr.ecr.ap-southeast-1.amazonaws.com/redis-cluster-init:latest
#docker push 701969852130.dkr.ecr.ap-southeast-1.amazonaws.com/redis-cluster-init:latest
docker tag asp.netcoreapistarter:latest khteh/asp.netcoreapistarter:latest
docker push khteh/asp.netcoreapistarter:latest
