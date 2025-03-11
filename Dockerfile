FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV NODE_VERSION 18.20.4
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz
RUN wget "$NODE_DOWNLOAD_URL" -O nodejs.tar.gz \
	&& tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
	&& rm nodejs.tar.gz \
	&& ln -s /usr/local/bin/node /usr/local/bin/nodejs \
	&& curl -sL https://deb.nodesource.com/setup_16.x |  bash - \
	&& apt update \
	&& apt-get install -y nodejs
WORKDIR /src
COPY ["AndrewLarsson.CircleOfTrust.Host/AndrewLarsson.CircleOfTrust.Host.csproj", "AndrewLarsson.CircleOfTrust.Host/"]
RUN dotnet restore "AndrewLarsson.CircleOfTrust.Host/AndrewLarsson.CircleOfTrust.Host.csproj"
COPY . .
WORKDIR "/src/AndrewLarsson.CircleOfTrust.Host"
RUN dotnet build "AndrewLarsson.CircleOfTrust.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AndrewLarsson.CircleOfTrust.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AndrewLarsson.CircleOfTrust.Host.dll"]
