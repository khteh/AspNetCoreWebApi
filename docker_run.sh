#!/bin/bash
docker pull khteh/asp.netcoreapistarter:latest
docker run -d -p5000:5000 khteh/asp.netcoreapistarter:latest
