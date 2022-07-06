/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: ChangePasswordDtos.cs
 * Project name | Nazwa Projektu: asp-net-po-schedule-management-server
 *
 * Klient | Client: <https://github.com/Milosz08/Angular_PO_Schedule_Management_Client>
 * Serwer | Server: <https://github.com/Milosz08/ASP.NET_PO_Schedule_Management_Server>
 *
 * RestAPI for the Angular application to manage schedule for sample university. Written with the ASP.NET Core
 * and Entity Framework with mySQL database. Project for the teaching course "Objected Oriented Programming".
 *
 * RestAPI dla aplikacji Angular do zarządzania planem zajęć przykładowej uczelni wyższej. Napisane w oparciu o
 * ASP.NET Core oraz Entity Framework z bazą danych mySQL. Projekt wykonany na zajęcia "Programowanie Obiektowe".
 */

using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "Pole poprzedniego hasła nie może być puste")]
        public string OldPassword { get; set; }
        
        [Required(ErrorMessage = "Pole nowego hasła nie może być puste")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",
            ErrorMessage = "Hasło musi mieć minimum 8 znaków, zawierać co najmniej jedną liczbę, " +
                           "jedną wielką literę oraz jeden znak specjalny.")]
        public string NewPassword { get; set; }
        
        [Required(ErrorMessage = "Pole potwierdzenia nowego hasła nie może być puste")]
        [Compare(nameof(NewPassword), ErrorMessage = "Hasła w obu polach muszą być identyczne.")]
        public string NewPasswordConfirmed { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class SetResetPasswordRequestDto
    {
        
        [Required(ErrorMessage = "Pole nowego hasła nie może być puste")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",
            ErrorMessage = "Hasło musi mieć minimum 8 znaków, zawierać co najmniej jedną liczbę, " +
                           "jedną wielką literę oraz jeden znak specjalny.")]
        public string newPassword	 { get; set; }
        
        [Required(ErrorMessage = "Pole potwierdzenia nowego hasła nie może być puste")]
        [Compare(nameof(newPassword), ErrorMessage = "Hasła w obu polach muszą być identyczne.")]
        public string newPasswordConfirmed { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class SetNewPasswordViaEmailResponse
    {
        public string Email { get; set; }
        public string BearerToken { get; set; }
    }
}