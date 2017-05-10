FROM microsoft/dotnet:2.0.0-preview1-sdk

COPY . /code

WORKDIR /code

RUN ls -al

RUN dotnet restore Botwin.sln

RUN ls -al

RUN dotnet build Botwin.sln

RUN dotnet test test/Botwin.Tests.csproj

WORKDIR /code