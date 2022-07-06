/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: ValidValuesAttribute.cs
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

using System.Linq;
using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.CustomDecorators
{
    public class ValidValuesAttribute : ValidationAttribute
    {
        private string[] _args;

        public ValidValuesAttribute(params string[] args)
        {
            _args = args;
        }

        /// <summary>
        /// Metoda nadpisująca domyślny dekorator do walidacji na dekorator przyjmujący wiele parametrów
        /// w postaci tablicy stringów (tablica musi być pre-kompilowana, czyli albo const albo na stałe zapisana).
        /// </summary>
        /// <param name="value">wartość sprawdzana</param>
        /// <param name="validationContext">kontekst walidacji</param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (_args.Contains(value as string)) {
                return ValidationResult.Success;
            }
            return new ValidationResult("Podana wartość nie jest zadeklarowana jako wartość akceptowalna");
        }
    }
}