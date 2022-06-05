<h1 align="center">
    System układania planów
    <br>
    <sup><sup>Schedule management system</sup></sup>
    <br>
    <img src="https://cdn.miloszgilga.pl/schedule-management-github-app-logo.png" width="140" height="140">
</h1>

> Demo dostępne pod adresem / demo available on: [schedule.miloszgilga.pl](https://schedule.miloszgilga.pl/) <br>
> Klient / client: [Angular PO Schedule Management Client](https://github.com/Milosz08/Angular_PO_Schedule_Management_Client) <br>

Projekt ma za zadanie symulować system zarządzania planem zajęć przykładowej wyższej uczelni technicznej. Aplikacja podzielona jest na klika głównych modułów z których można wyróżnić: moduł panelu administratora, moduł edytora planu oraz moduł publiczny.<br>
> The aim of this project is to simulate a schedule management system for an example technical university. The application is divided into several main modules: admin panel module, schedule editor module and public module. <br>

## Zastosowane technologie / technology stack
- Warstwa klienta / front-end layer:
    - [Angular Framework](https://angular.io/)
    - [TypeScript](https://www.typescriptlang.org/)
    - [RXJS](https://rxjs.dev/)
    - [NGRX](https://ngrx.io/)
- Warstwa serwera / back-end layer:
    - [ASP.NET Web API](https://dotnet.microsoft.com/en-us/apps/aspnet)
    - [Entity Framework](https://docs.microsoft.com/pl-pl/ef/)
    - [MySQL](https://www.mysql.com/)
    - [SSH.NET](https://github.com/sshnet/SSH.NET)
- Pozostałe usługi / middleware services:     
    - [JWT](https://jwt.io/)
    - [OAuth2](https://oauth.net/2/)
    - [Docker](https://www.docker.com/)

## Konta / accounts
Administrator systemu ma możliwość generowania kont, przy którym następuje dynamicznie generowana skrzynka pocztowa (przy użyciu protokołu SSH oraz serwera Nginx z zainstalowanym serwerem poczty SMTP/IMAP). Moderator może natomiast edytować plany na podstawie przypisania przez administratora do konkretnego wydziału. Nauczyciele i studenci mają możliwość wysyłania rezerwacji oraz ankiet wspomagających układanie planów.<br>
> The system administrator can generate accounts with a dynamically generated mailbox (using SSH protocol and Nginx server with SMTP/IMAP mail server installed). The moderator, on the other hand, can edit schedules based on assignment by the administrator to specific faculty. Teachers and students have the ability to send reservations and surveys to assist in scheduling.

...
