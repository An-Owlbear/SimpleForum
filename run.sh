#!/bin/bash

screen -S SimpleForum.API -dm bash -c "cd SimpleForum.API; dotnet SimpleForum.API.dll";
screen -S SimpleForum.CrossConnection -dm bash -c "cd SimpleForum.CrossConnection; dotnet SimpleForum.CrossConnection.dll";
screen -S SimpleForum.Web -dm bash -c "cd SimpleForum.Web; dotnet SimpleForum.Web.dll";