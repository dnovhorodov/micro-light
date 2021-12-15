ARG SDK_VERSION=6.0-alpine
ARG RUNTIME_VERSION=6.0-alpine

# Stage 1 - Build SDK image
FROM mcr.microsoft.com/dotnet/sdk:$SDK_VERSION AS build
WORKDIR /src
COPY ./src/sub ./sub
COPY ./src/shared ./shared
COPY ./src/utils ./utils

RUN dotnet restore ./sub -r alpine-x64
RUN dotnet publish ./sub/sub.fsproj -c Release -r alpine-x64 --self-contained true /p:PublishTrimmed=true --no-restore -o "../out"

# Stage 2 - Build runtime image
FROM mcr.microsoft.com/dotnet/runtime-deps:$RUNTIME_VERSION AS base
WORKDIR /micro-light-sub

ENV DOTNET_ENVIRONMENT Development
ENV MICROLIGHT_SERVER 127.0.0.1
ENV MICROLIGHT_PORT 5556

COPY --from=build /out ./
ENTRYPOINT ["./sub"]
