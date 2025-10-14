# Schedule Management Server

RestAPI for the [Angular application](https://github.com/milosz08/schedule-management-client) to manage schedules for
sample university. Written with the ASP.NET Core 8.0 and Entity Framework with MySQL database. Using AWS S3 SDK for
data storage and FluentEmail with Liquid templates for sending email messages.

[GitHub repository](https://github.com/milosz08/schedule-management-server)
| [Support](https://github.com/sponsors/milosz08)

## Build image

```bash
docker build -t milosz08/schedule-management-server .
```

## Create container

* Using command:

```bash
docker run -d \
  --name schedule-management-server \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Docker \
  -e SCHEDULE_MYSQL_URL="server=<>;port=3306;user=<>;password=<>;database=<>" \
  -e SCHEDULE_CLIENT_ORIGIN=<client application origin> \
  -e SCHEDULE_JWT_SECRET=<JWT secret> \
  -e SCHEDULE_SMTP_HOST=<schedule SMTP host> \
  -e SCHEDULE_SMTP_PORT=<schedule SMTP port> \
  -e SCHEDULE_SMTP_USERNAME=<schedule SMTP username> \
  -e SCHEDULE_SMTP_PASSWORD=<schedule SMTP password> \
  -e SCHEDULE_SMTP_ENABLE_SSH=<schedule SMTP SSH enabled> \
  -e SCHEDULE_SMTP_EMAIL_DOMAIN=<schedule SMTP email domain> \
  -e SCHEDULE_S3_HOST=<AWS S3 host> \
  -e SCHEDULE_S3_REGION=<AWS S3 region> \
  -e SCHEDULE_S3_ACCESS_KEY=<AWS S3 access key> \
  -e SCHEDULE_S3_SECRET_KEY=<AWS S3 secret key> \
  -e SCHEDULE_SSH_ENABLED=<decided, if mailbox management via SSH is enabled (true/false)> \
  -e SCHEDULE_SSH_HOST=<optional, SSH host> \
  -e SCHEDULE_SSH_LOGIN=<optional, SSH login> \
  -e SCHEDULE_SSH_PASSWORD=<optional, SSH password> \
  -e SCHEDULE_MAILBOX_CREATE_CMD=<optional, SSH create mailbox command, takes email address and password parameters> \
  -e SCHEDULE_MAILBOX_SET_CAPACITY_CMD=<optional, SSH set capacity mailbox command, takes email address> \
  -e SCHEDULE_MAILBOX_DELETE_CMD=<optional, SSH delete mailbox command, takes email address> \
  -e SCHEDULE_INIT_NAME=<init admin account name> \
  -e SCHEDULE_INIT_SURNAME=<init admin account surname> \
  -e SCHEDULE_INIT_PASSWORD=<init admin account password (raw)> \
  milosz08/schedule-management-server:latest
```

* Using `docker-compose.yml` file:

```yaml
services:
  schedule-management-server:
    container_name: schedule-management-server
    image: milosz08/schedule-management-server:latest
    ports:
      - '8080:8080'
    environment:
      ASPNETCORE_ENVIRONMENT: Docker
      SCHEDULE_CLIENT_ORIGIN: <client application origin>
      SCHEDULE_JWT_SECRET: <JWT secret>
      # db
      SCHEDULE_MYSQL_URL: server=<>;port=3306;user=<>;password=<>;database=<>
      # smtp
      SCHEDULE_SMTP_HOST: <schedule SMTP host>
      SCHEDULE_SMTP_PORT: <schedule SMTP port>
      SCHEDULE_SMTP_USERNAME: <schedule SMTP username>
      SCHEDULE_SMTP_PASSWORD: <schedule SMTP password>
      SCHEDULE_SMTP_ENABLE_SSH: <schedule SMTP SSH enabled>
      SCHEDULE_SMTP_EMAIL_DOMAIN: <schedule SMTP email domain>
      # s3
      SCHEDULE_S3_HOST: <AWS S3 host>
      SCHEDULE_S3_REGION: <AWS S3 region>
      SCHEDULE_S3_ACCESS_KEY: <AWS S3 access key>
      SCHEDULE_S3_SECRET_KEY: <AWS S3 secret key>
      # ssh and mailbox management
      SCHEDULE_SSH_ENABLED: <decided, if mailbox management via SSH is enabled (true/false)>
      SCHEDULE_SSH_HOST: <optional, SSH host>
      SCHEDULE_SSH_LOGIN: <optional, SSH login>
      SCHEDULE_SSH_PASSWORD: <optional, SSH password>
      SCHEDULE_MAILBOX_CREATE_CMD: <optional, SSH create mailbox command, takes email address and password parameters>
      SCHEDULE_MAILBOX_SET_CAPACITY_CMD: <optional, SSH set capacity mailbox command, takes email address>
      SCHEDULE_MAILBOX_DELETE_CMD: <optional, SSH delete mailbox command, takes email address>
      # init account
      SCHEDULE_INIT_NAME: <init admin account name>
      SCHEDULE_INIT_SURNAME: <init admin account surname>
      SCHEDULE_INIT_PASSWORD: <init admin account password (raw)>
    networks:
      - schedule-management-network

  # other containers...

networks:
  schedule-management-network:
    driver: bridge
```

## Author

Created by Miłosz Gilga. If you have any questions about this application, send
message: [miloszgilga@gmail.com](mailto:miloszgilga@gmail.com).

## License

This project is licensed under the Apache 2.0 License.
