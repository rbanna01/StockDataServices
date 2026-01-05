# Restore and build code
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
LABEL org.opencontainers.image.authors="Ruaridhi Bannatyne"
WORKDIR /source

# Restore project before copying rest
COPY --link ./Persistence/*.csproj .
RUN dotnet restore

# Copy code and publish
COPY --link ./Persistence/. .
RUN dotnet publish -o /publish

RUN --mount=type=secret,id=mongodb_connection_string \
env=MONGODB_CONNECTION_STRING

# Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /publish
COPY --link --from=build /publish .
EXPOSE 5210 
ENV ASPNETCORE_URLS=http://+:5210
ENTRYPOINT ["./Persistence"]
