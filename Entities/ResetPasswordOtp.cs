using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("reset-password-otp")]
    public class ResetPasswordOtp : PrimaryKeyEntityInjection
    {
        [Required]
        [Column("user-email")]
        public string Email { get; set; }
        
        [Required]
        [Column("user-otp")]
        public string Otp { get; set; }
        
        [Required]
        [Column("otp-expired")]
        public DateTime OtpExpired { get; set; }
        
        [Required]
        [Column("if-used")]
        public bool IfUsed { get; set; }
        
        [ForeignKey(nameof(Person))]
        [Column("person-key")]
        public long PersonId { get; set; }
        
        public virtual Person Person { get; set; }
    }
}