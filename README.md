<p align="center">
    <img src=".github/banner.png" alt="">
</p>

# Schedule Management Server

[[Docker image](https://hub.docker.com/r/milosz08/schedule-management-server)] |
[[About project](https://miloszgilga.pl/project/schedule-management-system)]

RestAPI for the [Angular application](https://github.com/milosz08/schedule-management-client) to manage schedules for
sample university. Written with the ASP.NET Core 8.0 and Entity Framework with MySQL database. Using AWS S3 SDK for
data storage and FluentEmail with Liquid templates for sending email messages.

## Table of content

* [Clone and run](#clone-and-run)
* [Prepare development environment](#prepare-development-environment)
* [Tech stack](#tech-stack)
* [Author](#author)
* [License](#license)

## Clone and run

1. Clone repository on your local machine via:

```bash
git clone https://github.com/milosz08/schedule-management-server
```

2. Create `.env` file and fill up environment variables based on `example.env` file:

```properties
SCHEDULE_DEV_MYSQL_PORT=7690
SCHEDULE_DEV_MAILHOG_API_PORT=7691
SCHEDULE_DEV_MAILHOG_UI_PORT=7692
SCHEDULE_DEV_S3_API_PORT=7693
SCHEDULE_DEV_S3_UI_PORT=7694
SCHEDULE_DEV_API_PORT=7695
SCHEDULE_DEV_CLIENT_PORT=7696

SCHEDULE_DEV_JWT_SECRET=<JWT secret>
# db
SCHEDULE_DEV_MYSQL_USERNAME=<MySQL database username>
SCHEDULE_DEV_MYSQL_PASSWORD=<MySQL database password>
SCHEDULE_DEV_MYSQL_DB_NAME=<MySQL database name>
# smtp
SCHEDULE_DEV_MAILHOG_USERNAME=<Mailhog username, by default mailhoguser (see .volumes/mail/mailhog-auth.txt file)>
SCHEDULE_DEV_MAILHOG_PASSWORD=<Mailhog password, by default root (see .volumes/mail/mailhog-auth.txt file)>
SCHEDULE_DEV_SSH_ENABLED=<decided, if mailbox management via SSH is enabled (true/false)>
# S3
SCHEDULE_DEV_S3_USERNAME=<S3 username>
SCHEDULE_DEV_S3_PASSWORD=<S3 password>
# ssh and mailbox management
SCHEDULE_DEV_SSH_HOST=<optional, SSH host>
SCHEDULE_DEV_SSH_LOGIN=<optional, SSH login>
SCHEDULE_DEV_SSH_PASSWORD=<optional, SSH password>
SCHEDULE_DEV_MAILBOX_CREATE_CMD=<optional, SSH create mailbox command, takes email address and password parameters>
SCHEDULE_DEV_MAILBOX_SET_CAPACITY_CMD=<optional, SSH set capacity mailbox command, takes email address>
SCHEDULE_DEV_MAILBOX_DELETE_CMD=<optional, SSH delete mailbox command, takes email address>
# init account
SCHEDULE_DEV_INIT_NAME=<init admin account name>
SCHEDULE_DEV_INIT_SURNAME=<init admin account surname>
SCHEDULE_DEV_INIT_PASSWORD=<init admin account password (raw)>
```

3. Run all containers via:

```bash
$ docker compose up -d
```

This command should create following containers:

| Name                             | Port(s)         | Link                                    |
|----------------------------------|-----------------|-----------------------------------------|
| schedule-management-mysql-db     | 7690            | [localhost:7690](http://localhost:7690) |
| schedule-management-mailhog-smtp | 7691, 7692 (UI) | [localhost:7692](http://localhost:7692) |
| schedule-management-minio-s3     | 7693, 7694 (UI) | [localhost:7694](http://localhost:7694) |
| schedule-management-app          | 7695            | [localhost:7695](http://localhost:7695) |

## Prepare development environment

1. Make sure that you have [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed on your machine.
2. Create `.env` file and fill up environment variables based on `example.env` file.
3. Run necessary containers via:

```bash
$ docker compose up -d \
  schedule-management-mysql-db \
  schedule-management-mailhog-smtp \
  schedule-management-minio-s3
```

This command should create following containers:

| Name                             | Port(s)         | Link                                    |
|----------------------------------|-----------------|-----------------------------------------|
| schedule-management-mysql-db     | 7690            | [localhost:7690](http://localhost:7690) |
| schedule-management-mailhog-smtp | 7691, 7692 (UI) | [localhost:7692](http://localhost:7692) |
| schedule-management-minio-s3     | 7693, 7694 (UI) | [localhost:7694](http://localhost:7694) |

4. Run project in debug mode via:

```bash
$ ASPNETCORE_ENVIRONMENT=Development \
  ASPNETCORE_URLS=http://localhost:7695 \
  dotnet run --project ./ScheduleManagement/ScheduleManagement.csproj
```

5. Alternatively you can run via prepared Rider configuration in `.run` directory.

## Tech stack

* ASP.NET 8.0,
* Entity Framework, MySql,
* FluentEmail, Liquid templates,
* ImageSharp,
* SSH.NET,
* AWSSDK,
* JwtBearer, NewtonsoftJson,
* OpenApi,
* Docker and Docker compose.

## Author

Created by Miłosz Gilga. If you have any questions about this application, send
message: [miloszgilga@gmail.com](mailto:miloszgilga@gmail.com).

## License

This project is licensed under the Apache 2.0 License.
