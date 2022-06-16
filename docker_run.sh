#!/bin/bash
docker pull khteh/asp.netcorewebapi:latest
docker run -d -p5000:5000 khteh/asp.netcorewebapi:latest
