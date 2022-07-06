/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: UserQueryValidator.cs
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
using FluentValidation;

using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.Dto.Validators
{
    /// <summary>
    /// Walidator odpowiedzialny za walidację elementów paginacji
    /// </summary>
    public class UserQueryValidator : AbstractValidator<SearchQueryRequestDto>
    {
        // możliwe sortowania elementów (do wybory przez użytkownika)
        private string[] _allowedSortings =
        {
            "Id", "Surname", "Login", "Role", "Name", "Alias", "DepartmentName", "DepartmentAlias", "CathedralAlias",
            "Capacity", "RoomTypeAlias", "SpecTypeName", "SpecDegree", "SpecTypeAlias", "IssueType", "IfAnonymous",
            "CreatedDate",
        };
        
        public UserQueryValidator()
        {
            RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(q => q.PageSize).Custom((value, context) => {
                if (!ApplicationUtils._allowedPageSizes.Contains(value)) {
                    context.AddFailure(
                        "PageSize",
                        $"Ilość elementów to: [{string.Join(",", ApplicationUtils._allowedPageSizes)}]");
                }
            });
            RuleFor(p => p.SortBy)
                .Must(v => string.IsNullOrEmpty(v) || _allowedSortings.Contains(v))
                .WithMessage(
                    $"Sortowanie jest opcjonalne, ale trzeba podać jedne z: [{string.Join(",", _allowedSortings)}]");
        }
    }
}