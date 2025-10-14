FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ScheduleManagement/*.csproj ./
RUN dotnet restore

COPY ScheduleManagement/. ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/out .

LABEL maintainer="Miłosz Gilga <miloszgilga@gmail.com>"

EXPOSE 8080
ENTRYPOINT ["dotnet", "ScheduleManagement.dll"]
