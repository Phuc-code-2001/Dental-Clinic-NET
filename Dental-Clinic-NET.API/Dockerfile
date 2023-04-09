#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Dental-Clinic-NET.API/Dental-Clinic-NET.API.csproj", "Dental-Clinic-NET.API/"]
COPY ["DataLayer/DataLayer.csproj", "DataLayer/"]
COPY ["FileProcessorServices/FileProcessorServices.csproj", "FileProcessorServices/"]
COPY ["ImageProcessLayer/ImageProcessLayer.csproj", "ImageProcessLayer/"]
COPY ["MailServices/MailServices.csproj", "MailServices/"]
COPY ["RealTimeProcessLayer/RealTimeProcessLayer.csproj", "RealTimeProcessLayer/"]
COPY ["SegementationXRayServices/SegementationXRayServices.csproj", "SegementationXRayServices/"]
RUN dotnet restore "Dental-Clinic-NET.API/Dental-Clinic-NET.API.csproj"
COPY . .
WORKDIR "/src/Dental-Clinic-NET.API"
RUN dotnet build "Dental-Clinic-NET.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dental-Clinic-NET.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dental-Clinic-NET.API.dll"]