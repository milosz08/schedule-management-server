using System.Linq;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.DbConfig
{
    public class ApplicationDbSeeder
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbSeeder(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // funkcja seedująca dane do bazy danych
        public void Seed()
        {
            if (_context.Database.CanConnect()) {
                // wywoływane jeśli przy migracji nie ma żadnych elementów w encji "roles" (auto-seedowanie)
                if (!_context.Roles.Any()) {
                    _context.Roles.AddRange(InsertRoles());
                    _context.SaveChanges();
                }
            }
        }

        // umieszczanie wszystkich możliwych ról (na podstawie enuma) w liście.
        private IEnumerable<Role> InsertRoles()
        {
            List<Role> allRoles = new List<Role>();
            IEnumerable<string> possibleRoles = System.Enum.GetNames(typeof(AvailableRoles));
            foreach (var singleRole in possibleRoles) {
                allRoles.Add(new Role() { Name = singleRole });
            }
            return allRoles;
        }
    }
}