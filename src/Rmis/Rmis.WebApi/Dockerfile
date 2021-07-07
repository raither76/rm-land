FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Rmis.WebApi/Rmis.WebApi.csproj", "Rmis.WebApi/"]
RUN dotnet restore "Rmis.WebApi/Rmis.WebApi.csproj"
COPY . .
WORKDIR "/src/Rmis.WebApi"
RUN dotnet build "Rmis.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rmis.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rmis.WebApi.dll"]
