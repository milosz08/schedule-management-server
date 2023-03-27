# SUoT Schedule Management System (Backend)
[![Generic badge](https://img.shields.io/badge/Made%20with-ASP.NET%20Core%205.0-1abc9c.svg)](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-5.0)&nbsp;&nbsp;
[![Generic badge](https://img.shields.io/badge/Build%20with-Docker%20Composer-green.svg)](https://gradle.org/)&nbsp;&nbsp;
[![Generic badge](https://img.shields.io/badge/Packaging-Docker%20Container-brown.svg)](https://gradle.org/)&nbsp;&nbsp;
<br><br>
The aim of this project is to simulate a schedule management system for an example technical university. The application is divided into several main modules: admin panel module, schedule editor module and public module. The project was created as part of the credit for the course "Object Oriented Programming" during the pursuit of an engineering degree in Computer Science. <br>

See live application at: [schedule.miloszgilga.pl](https://schedule.miloszgilga.pl)<br>
See frontend (client layer): [SUoT Schedule Management Client](https://github.com/Milosz08/SUoT_Schedule_Management_Client)

## Technology stack
-  Front-end layer:
    - [Angular Framework](https://angular.io/)
    - [TypeScript](https://www.typescriptlang.org/)
    - [RXJS (Reactive JS)](https://rxjs.dev/)
    - [NGRX (Flux store)](https://ngrx.io/)
-  Back-end layer:
    - [ASP.NET Web API](https://dotnet.microsoft.com/en-us/apps/aspnet)
    - [Entity Framework](https://docs.microsoft.com/pl-pl/ef/)
    - [MySQL](https://www.mysql.com/)
    - [SSH.NET](https://github.com/sshnet/SSH.NET)
- Other services:     
    - [JWT (JSON Web Token)](https://jwt.io/)
    - [OAuth2 (JWT Assertion)](https://oauth.net/2/)
    - [Docker](https://www.docker.com/)

## Clone, prepare and run

> Note: Before starting the server, you need to configure a server that supports SSH and SMTP or buy such a server from AWS or MS Azure.

- To install the program on your computer use the command (or use the built-in GIT system in your IDE environment):
```
$ git clone https://github.com/Milosz08/schedule-management-server
```
- Create `.env` file (on LOCAL machine) or add via prompt or web interface (Azure) following environment variables:
```properties
MY_SEQUEL_CONNECTION        = xxxxx <- database connection string
JWT_KEY                     = xxxxx <- JSON Web Token secred key (ex. dagtg4r382378f7f8ed78fn8928f7n)
SSH_USERNAME                = xxxxx <- SSH connection username
SSH_PASSWORD                = xxxxx <- SSH connection password
SSH_SERVER                  = xxxxx <- SSH connection server
SMTP_USERNAME               = xxxxx <- SMTP mail server username
SMTP_PASSWORD               = xxxxx <- SMTP mail server password
SMTP_HOST                   = xxxxx <- SMTP mail server host agent
INITIAL_ACCOUNT_NAME        = xxxxx <- user name (ex. John)
INITIAL_ACCOUNT_SURNAME     = xxxxx <- user surname (ex. Black)
INITIAL_ACCOUNT_PASSWORD    = xxxxx <- user password (non hash form, ex. ThisIsSecretPassword123#)
```
The contents of the `appsettings.json` file located in the root folder of the project:
- You can edit the rest of the settings in the `appsettings.json file`.
> Attention: If you provide invalid parameters or there are no parameters in the `appsettings.json` file, the server will not start correctly.

- Edit `SshEmailServiceImplementation.cs` file:
```c#
...
public void AddNewEmailAccount(string emailAddress, string emailPassword)
{
    _sshInterceptor.ExecuteCommand(
        // insert your ShellScript command to create email accounts on SMTP server
    );
    _sshInterceptor.ExecuteCommand(
        // insert your ShellScript command to added maximum size of created maibox
    );
}

public void UpdateEmailPassword(string emailAddress, string newEmailPassword)
{
    _sshInterceptor.ExecuteCommand(
       // insert your ShellScript command to update email account password
    );
}

public void DeleteEmailAccount(string emailAddress)
{
    _sshInterceptor.ExecuteCommand(
        // insert your ShellScript command to delete email account
    );
}
...
```

## Swagger documentation

Swagger documentation has been prepared in the application. To access the documentation, just go to
```
http://127.0.0.1:[appPort]/swagger/index.html
```
where `appPort` can be, for example 7575.
