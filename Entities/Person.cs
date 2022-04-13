using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using asp_net_po_schedule_management_server.Entities.Shared;

namespace asp_net_po_schedule_management_server.Entities
{
    [Table("person-table")]
    public class Person : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [Column("name")]
        [StringLength(50, ErrorMessage = "Pole imienia użytkownika nie może przekraczać 50 znaków")]
        public string Name { get; set; }
        
        [Required]
        [Column("surname")]
        [StringLength(50, ErrorMessage = "Pole nazwiska użytkownika nie może przekraczać 50 znaków")]
        public string Surname { get; set; }
        
        [Column("shortcut")]
        [StringLength(8, ErrorMessage = "Pole skrótu użytkownika nie może przekraczać 8 znaków")]
        public string Shortcut { get; set; }
        
        [Required]
        [Column("app-login")]
        public string AppLogin { get; set; }
        
        [Column("app-password")]
        public string AppPassword { get; set; }
        
        [Required]
        [Column("email")]
        public string Email { get; set; }
        
        [Required]
        [Column("nationality")]
        [StringLength(50, ErrorMessage = "Pole pochodzenia użytkownika nie może przekraczać 50 znaków")]
        public string Nationality { get; set; }
        
        [Required]
        [Column("city")]
        [StringLength(100, ErrorMessage = "Pole miejscowości użytkownika nie może przekraczać 100 znaków")]
        public string City { get; set; }

        [ForeignKey("role-key")]
        public int RoleForeignKey { get; set; }
        
        public virtual Role role { get; set; }
    }
}