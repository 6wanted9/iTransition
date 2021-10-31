#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["task4/task4.csproj", "task4/"]
RUN dotnet restore "task4/task4.csproj"
COPY . .
WORKDIR "/src/task4"
RUN dotnet build "task4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "task4.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "task4.dll"]