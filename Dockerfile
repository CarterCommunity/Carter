FROM microsoft/dotnet:1.1.1-sdk

COPY . /code

RUN dotnet restore Botwin.sln

RUN dotnet build Botwin.sln

RUN dotnet test test/Botwin.Tests.csproj

WORKDIR /code