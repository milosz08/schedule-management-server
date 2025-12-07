FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ScheduleManagement/*.csproj ./
RUN dotnet restore

COPY ScheduleManagement/. ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

ENV ASPNETCORE_HTTP_PORTS=8080

WORKDIR /app

COPY --chown=app:app --from=build /app/out .

LABEL maintainer="Miłosz Gilga <miloszgilga@gmail.com>"

EXPOSE 8080

USER app

ENTRYPOINT ["dotnet", "ScheduleManagement.dll"]
