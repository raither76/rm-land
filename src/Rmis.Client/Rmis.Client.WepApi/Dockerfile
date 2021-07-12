FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Rmis.Client.WepApi/Rmis.Client.WepApi.csproj", "Rmis.Client.WepApi/"]
RUN dotnet restore "Rmis.Client.WepApi/Rmis.Client.WepApi.csproj"
COPY . .
WORKDIR "/src/Rmis.Client.WepApi"
RUN dotnet build "Rmis.Client.WepApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rmis.Client.WepApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rmis.Client.WepApi.dll"]
