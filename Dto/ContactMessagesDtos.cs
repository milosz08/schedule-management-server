using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class ContactMessagesReqDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string UserHash { get; set; }
        public string DepartmentName { get; set; }
        public List<long> Groups { get; set; }
        
        [Required(ErrorMessage = "Pole typu zgłoszenia nie może być puste")]
        public string IssueType { get; set; }
        
        [Required(ErrorMessage = "Pole opisu zgłoszenia nie może być puste")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Pole statusu zalogowania przy zgłoszeniu nie może być puste")]
        public bool IfAnonymous { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ContactMessagesQueryResponseDto
    {
        public long Id { get; set; }
        public string NameWithSurname { get; set; }
        public string IssueType { get; set; }
        public bool IfAnonymous { get; set; }
        public string CreatedDate { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class SingleContactMessageResponseDto
    {
        public string NameWithSurname { get; set; }
        public string Email { get; set; }
        public string MessageIdentifier { get; set; }
        public string IssueType { get; set; }
        public string DepartmentName { get; set; } = "brak";
        public List<string> Groups { get; set; } = new List<string>();
        public bool IfAnonymous { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
    }
}