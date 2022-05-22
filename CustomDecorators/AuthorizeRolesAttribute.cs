using Microsoft.AspNetCore.Authorization;


namespace asp_net_po_schedule_management_server.CustomDecorators
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Metoda zmieniająca działanie domyślnego dekoratora Authorize, umożliwiając przesłanie
        /// wartości innych niż stringi (w tym wypadku enumy) w jednym ciągu.
        /// </summary>
        /// <param name="allowedRoles">tablica parametrów dozwolonych ról</param>
        public AuthorizeRolesAttribute(params string[] allowedRoles)
        {
            Roles = string.Join(",", allowedRoles);
        }
    }
}