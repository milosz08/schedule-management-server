<h1 align="center">
    Schedule management system
    <br>
    <img src="https://cdn.miloszgilga.pl/schedule-management-project-logo.png" width="140">
</h1>

> Demo available on: [schedule.miloszgilga.pl](https://schedule.miloszgilga.pl/) <br>
> Client: [Angular PO Schedule Management Client](https://github.com/Milosz08/Angular_PO_Schedule_Management_Client) <br>

The aim of this project is to simulate a schedule management system for an example technical university. The application is divided into several main modules: admin panel module, schedule editor module and public module. The project was created as part of the credit for the course "Object Oriented Programming" during the pursuit of an engineering degree in Computer Science. <br>

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
$ git clone https://github.com/Milosz08/ASP.NET_PO_Schedule_Management_Server
```
- Create `appsettings.json` and insert services connections:
```
$ touch appsettings.json
```
The contents of the `appsettings.json` file located in the root folder of the project:
```js
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft": "Warning",
            "Microsoft.Hosting.Lifetime": "Information"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "MySequelConnection": "server=[dbServer];port=3306;user=[dbUser];password=[dbPassword];database=[dbName]"
    },
    "ServerConfiguration": {
        "ClientOrigin": "",             // deployment client URL address (ex. https://schedule.pl)
        "DevClientOrigin": "",          // development client URL address (ex. http://127.0.0.1:8383)
        "JwtKey": "",                   // JSON Web Token secred key (ex. dagtg4r382378f7f8ed78fn8928f7n)
        "JwtExpiredTimestamp": "",      // Time after JSON Web Token is expired (ex. 00:24:00)
        "DbDriverVersion": "",          // database driver version (ex. 8.0.29)
        "SshUsername": "",              // SSH connection username
        "SshPassword": "",              // SSH connection password
        "SshPasswordFieldName": "",     // SSH password indicator value (ex. Password)
        "SshServer": "",                // SSH server (ex. s5.aws.net)
        "UserEmailDomain": "",          // SMTP email domain (ex. schedule.pl)
        "UserEmailMaxSizeMb": 20,       // Maximum maibox size (in Mb, ex. 20)
        "OptExpired": "",               // OPT token expired timestamp (ex. 00:10:00)
        "SmtpSenderAddress": "",        // auto-responder email URL address (ex. noreply@schedule.pl)
        "SmtpSenderDisplayName": "",    // auto-responder email header (ex. Schedule Management System)
        "SmtpUsername": "",             // SMTP server username
        "SmtpPassword": "",             // SMTP server password
        "SmtpHost": "",                 // SMTP server URL address (ex. mail.schedule.net)
        "SmtpPort": 587,                // SMTP port (by default 587)
        "EnableSSL": true,              // if SSL in emails should be enabled (by default true)
        "UseDefaultCredentials": true,  // if use default credentials in SMTP service (by default true)
        "IsBodyHTML": true,             // if emails can have HTML body (by default true)
        "InitialCredentials": {         // initial account creating after create migrations
            "AccountName": "",          // user name (ex. John)
            "AccountSurname": "",       // user surname (ex. Black)
            "AccountPassword": ""       // user password (non hash form, ex. ThisIsSecretPassword123#)
        }
    }
}
```
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

## About the project

### Accounts
The system administrator can generate accounts with a dynamically generated mailbox (using SSH protocol and Nginx server with SMTP/IMAP mail server installed). The moderator, on the other hand, can edit schedules based on assignment by the administrator to specific faculty. Teachers and students have the ability to send reservations and surveys to assist in scheduling. After the first login, the system allows you to change the password from the default one (generated by the server) to your own that meets the given criteria.<br><br>
There is also a page that allows the user to reset his password. This is done by providing an email address and the server sends a token (short-life token) which is valid for 10 minutes and can be used only once. After validating the token, the server returns a JWT which allows the user to enter a new password.<br><br>
The login page shows the accounts from which you last logged into the application. After clicking on the account icon, the application will automatically enter the email address. Accounts are stored using Local Storage mechanism. Each user can delete accounts/accounts from history. Additionally, after a successful login a sequencer counting down the time of active user session is present (based on data received from server). Any change made to the page resets the timer. If there is no activity on the page for N time, the application will start a 30 second sequencer giving the option to renew the session time or logout. If there is no response, the application is automatically logged out of the system after 30 seconds. 

### Schedule canvas generator page
In the schedule page it is possible to navigate by weeks (current week is displayed by default). It is also possible to navigate by academic year, then new week values will be generated and downloaded from server. After opening the plan (if local storage is enabled) it will be saved in the bookmark. This way, when you return to the page, you don't have to search in TreeView, but just click on the bookmark of the saved group.

### Content management system
Once successfully logged into the Content Management System (CMS), depending on the role assigned to the account, items can be added or deleted. The administrator can add departments, chairs, subjects, majors, classrooms and manage all user messages. Thanks to storing all elements in the system and using relationships in the database, when creating a timetable you don't have to enter subjects as new text fields, but just choose from the list of already created ones.<br><br>
The database relationship implementation is set to cascade deletion, i.e. if there is a deletion of element B with an element A relationship, element A will also be deleted. This counteracts nullable values and non-existent object references when the server is preparing a query. For each subpage where there may be a risk of recursive deletion of relationships, a message is displayed to warn the user. 

### Further application development
Thanks to the application's modularity (using the Angular framework), it is quite easy to add new functionalities to the application (e.g. reservation handling, remote education services, etc.). Also, thanks to the application of the server layer in the form of API interface, it is possible to create a mobile application (native in Kotlin for Android, or Swift for IOS, or cross-platform using e.g. Xamarin or Flutter) and in a fairly simple way connect it to the server by making available or creating additional controllers available for the mobile application using the HTTP/Websocket protocol and REST specification.<br><br>
In case of growing server layer complexity and high application traffic, one may be tempted to use microservices using CDN and message queuing broker under ASP.NET Core platform such as Apache Kafka.
