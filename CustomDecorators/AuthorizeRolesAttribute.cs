using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;

using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.CustomDecorators
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Metoda zmieniająca działanie domyślnego dekoratora Authorize, umożliwiając przesłanie
        /// wartości innych niż stringi (w tym wypadku enumy) w jednym ciągu.
        /// </summary>
        /// <param name="allowedRoles">tablica parametrów dozwolonych ról</param>
        public AuthorizeRolesAttribute(params AvailableRoles[] allowedRoles)
        {
            IEnumerable<string> allowedRolesAsStrings = allowedRoles
                .Select(role => Enum.GetName(typeof(AvailableRoles), role));
            Roles = string.Join(",", allowedRolesAsStrings);
        }
    }
}