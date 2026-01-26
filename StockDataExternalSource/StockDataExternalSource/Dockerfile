#syntax=docker/dockerfile:1.4

#Port and IP address to use
ARG PORT="5221"
ARG IPADDRESS="0.0.0.0"

#Restore and build
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
LABEL org.opencontainers.image.authors="Ruaridhi Bannatyne"

WORKDIR /source

#Restore
COPY --link ./*.csproj .
RUN dotnet restore

#Build release version
COPY --link ./. .
RUN dotnet publish -o /publish

#Run in image with runtime only
FROM mcr.microsoft.com/dotnet/aspnet:10.0
ARG PORT
ARG IPADDRESS
WORKDIR /publish
COPY --link --from=build /publish .
EXPOSE ${PORT}
ENV ASPNETCORE_URLS=http://${IPADDRESS}:${PORT}
ENTRYPOINT ["./StockDataExternalSource"]

# Running something like: docker run -p 5221:5221 --env MARKET_STACK_API_KEY=<APIKey> e2c29a118d049f1598ac1b205de043d9eddb8191367a6
