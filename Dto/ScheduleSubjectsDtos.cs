using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class ScheduleDataRes<T>
    {
        public List<string> TraceDetails { get; set; } = new List<string>();
        public string ScheduleHeaderData { get; set; }
        public List<ScheduleCanvasData<T>> ScheduleCanvasData { get; set; } = new List<ScheduleCanvasData<T>>();
    }

    public sealed class ScheduleCanvasData<T>
    {
        public NameWithDbIdElement WeekdayNameWithId { get; set; }
        public bool IfEmpty { get; set; }
        public List<T> WeekdayData { get; set; } = new List<T>();
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleGroups : BaseScheduleResData
    {
        public List<ScheduleTeacherQuery> TeachersAliases { get; set; } = new List<ScheduleTeacherQuery>();
        public List<ScheduleRoomQuery> RoomsAliases { get; set; } = new List<ScheduleRoomQuery>();
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleTeachers : BaseScheduleResData
    {
        public ScheduleGroupQuery StudyGroupAlias { get; set; }
        public List<ScheduleRoomQuery> RoomsAliases { get; set; } = new List<ScheduleRoomQuery>();
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleRooms : BaseScheduleResData
    {
        public ScheduleGroupQuery StudyGroupAlias { get; set; }
        public List<ScheduleTeacherQuery> TeachersAliases { get; set; } = new List<ScheduleTeacherQuery>();
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public abstract class BaseScheduleResData
    {
        public string SubjectWithTypeAlias { get; set; }
        public string SubjectTypeHexColor { get; set; }
        public string SubjectTime { get; set; }
        public int PositionFromTop { get; set; }
        public int ElementWidth { get; set; }
        public string SubjectOccuredData { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleGroupQuery
    {
        public long DeptId { get; set; }
        public long SpecId { get; set; }
        public long GroupId { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleTeacherQuery
    {
        public long DeptId { get; set; }
        public long CathId { get; set; }
        public long EmployeerId { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleRoomQuery
    {
        public long DeptId { get; set; }
        public long CathId { get; set; }
        public long RoomId { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleActivityReqDto
    {
        public long DeptId { get; set; }
        public long StudySpecId { get; set; }
        public long StudyGroupId { get; set; }
        public long WeekDayId { get; set; }
        public string SubjectOrActivityName { get; set; }
        public string SubjectTypeName { get; set; }
        public List<long> SubjectRooms { get; set; }
        public List<long> SubjectTeachers { get; set; }
        public string HourStart { get; set; }
        public string HourEnd { get; set; }
        public List<string> WeeksData { get; set; }
    }
}