using System;
using System.Linq;

using Microsoft.AspNetCore.Authorization;

using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.CustomDecorators
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        // metoda zmieniająca działanie domyślnego dekoratora Authorize, umożliwiając przesłanie
        // wartości innych niż stringi (w tym wypadku enumy) w jednym ciągu
        public AuthorizeRolesAttribute(params AvailableRoles[] allowedRoles)
        {
            var allowedRolesAsStrings = allowedRoles.Select(role => Enum.GetName(typeof(AvailableRoles), role));
            Roles = string.Join(",", allowedRolesAsStrings);
        }
    }
}