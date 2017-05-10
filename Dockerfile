FROM microsoft/dotnet:2.0.0-preview1-sdk

COPY . /code

WORKDIR /code

RUN dotnet restore Botwin.sln

RUN dotnet build Botwin.sln

RUN dotnet test test/Botwin.Tests.csproj
