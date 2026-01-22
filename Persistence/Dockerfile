# syntax=docker/dockerfile:1.4

#Port on which to listen
ARG PORT="5210"

# Restore and build code
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
LABEL org.opencontainers.image.authors="Ruaridhi Bannatyne"

WORKDIR /source

# Restore project before copying rest
COPY --link ./Persistence/*.csproj .
RUN dotnet restore

# Copy code and publish
COPY --link ./Persistence/. .
RUN dotnet publish -o /publish


# Run
FROM mcr.microsoft.com/dotnet/aspnet:10.0
ARG PORT
WORKDIR /publish
COPY --link --from=build /publish .
EXPOSE ${PORT} 
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["./Persistence"]
