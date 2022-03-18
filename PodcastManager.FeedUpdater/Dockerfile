FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

ENV MONGO_URL=localhost
ENV RABBIT_URL=localhost

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PodcastManager.FeedUpdater/PodcastManager.FeedUpdater.csproj", "PodcastManager.FeedUpdater/"]
RUN dotnet restore "PodcastManager.FeedUpdater/PodcastManager.FeedUpdater.csproj"
COPY . .
WORKDIR "/src/PodcastManager.FeedUpdater"
RUN dotnet build "PodcastManager.FeedUpdater.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PodcastManager.FeedUpdater.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PodcastManager.FeedUpdater.dll"]
