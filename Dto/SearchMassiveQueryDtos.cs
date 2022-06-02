namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class SearchMassiveQueryReqDto
    {
        public string SearchQuery { get; set; }
        public bool IfGroupsActive { get; set; }
        public bool IfTeachersActive { get; set; }
        public bool IfRoomsActive { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class SearchMassiveQueryResDto
    {
        public dynamic PathQueryParams { get; set; }
        public string DepartmentName { get; set; }
        public string PathParam { get; set; }
        public string TypeName { get; set; }
        public string FullName { get; set; }
    }
}