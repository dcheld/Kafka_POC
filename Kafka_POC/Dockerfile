FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Kafka_POC/Kafka_POC.csproj", "Kafka_POC/"]
RUN dotnet restore "Kafka_POC/Kafka_POC.csproj"
COPY . .
WORKDIR "/src/Kafka_POC"
RUN dotnet build "Kafka_POC.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Kafka_POC.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Kafka_POC.dll"]