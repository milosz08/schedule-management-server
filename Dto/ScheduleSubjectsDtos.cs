/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: ScheduelSubjectsDtos.cs
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

using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class ScheduleDataRes<T>
    {
        public List<string> TraceDetails { get; set; } = new List<string>();
        public string ScheduleHeaderData { get; set; }
        public string CurrentChooseWeek { get; set; }
        public List<ScheduleCanvasData<T>> ScheduleCanvasData { get; set; } = new List<ScheduleCanvasData<T>>();
    }

    public sealed class ScheduleCanvasData<T>
    {
        public NameWithDbIdElement WeekdayNameWithId { get; set; }
        public string WeekdayDateTime { get; set; }
        public bool IfNotShowingOccuredDates { get; set; } 
        public List<T> WeekdayData { get; set; } = new List<T>();
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public class ScheduleGroups : BaseScheduleResData
    {
        public List<ScheduleMultipleValues<ScheduleTeacherQuery>> TeachersAliases { get; set; }
        public List<ScheduleMultipleValues<ScheduleRoomQuery>> RoomsAliases { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public class ScheduleTeachers : BaseScheduleResData
    {
        public List<ScheduleMultipleValues<ScheduleGroupQuery>> StudyGroupAliases { get; set; } =
            new List<ScheduleMultipleValues<ScheduleGroupQuery>>();

        public List<ScheduleMultipleValues<ScheduleRoomQuery>> RoomsAliases { get; set; } =
            new List<ScheduleMultipleValues<ScheduleRoomQuery>>();
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleRooms : BaseScheduleResData
    {
        public List<ScheduleMultipleValues<ScheduleGroupQuery>> StudyGroupAliases { get; set; } =
            new List<ScheduleMultipleValues<ScheduleGroupQuery>>();

        public List<ScheduleMultipleValues<ScheduleTeacherQuery>> TeachersAliases { get; set; } =
            new List<ScheduleMultipleValues<ScheduleTeacherQuery>>();
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public class BaseScheduleResData
    {
        public long ScheduleSubjectId { get; set; }
        public string SubjectWithTypeAlias { get; set; }
        public string SubjectTypeHexColor { get; set; }
        public string SubjectTime { get; set; }
        public int PositionFromTop { get; set; }
        public int ElementHeight { get; set; }
        public string SubjectOccuredData { get; set; }
        public bool IfNotShowingOccuredDates { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleMultipleValues<T>
    {
        public string Alias { get; set; }
        public T PathValues { get; set; }

        public ScheduleMultipleValues(string alias, T pathValues)
        {
            Alias = alias;
            PathValues = pathValues;
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public class ScheduleGroupQuery
    {
        public long DeptId { get; set; }
        public long SpecId { get; set; }
        public long GroupId { get; set; }

        public ScheduleGroupQuery() { }

        public ScheduleGroupQuery(long deptId, long specId, long groupId)
        {
            DeptId = deptId;
            SpecId = specId;
            GroupId = groupId;
        }
    }

    //------------------------------------------------------------------------------------------------------------------

    public class ScheduleTeacherQuery
    {
        public long DeptId { get; set; }
        public long CathId { get; set; }
        public long EmployeerId { get; set; }

        public ScheduleTeacherQuery() { }

        public ScheduleTeacherQuery(long deptId, long cathId, long employeerId)
        {
            DeptId = deptId;
            CathId = cathId;
            EmployeerId = employeerId;
        }
    }

    //------------------------------------------------------------------------------------------------------------------

    public class ScheduleRoomQuery
    {
        public long DeptId { get; set; }
        public long CathId { get; set; }
        public long RoomId { get; set; }
        
        public ScheduleRoomQuery() { }

        public ScheduleRoomQuery(long deptId, long cathId, long roomId)
        {
            DeptId = deptId;
            CathId = cathId;
            RoomId = roomId;
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleActivityReqDto
    {
        public long DeptId { get; set; }
        public long StudySpecId { get; set; }
        public long StudyGroupId { get; set; }
        public bool IfAddForAllGroups { get; set; }
        public string StudyYear { get; set; }
        public long WeekDayId { get; set; }
        public string SubjectOrActivityName { get; set; }
        public string SubjectTypeName { get; set; }
        public List<long> SubjectRooms { get; set; }
        public List<long> SubjectTeachers { get; set; }
        public string HourStart { get; set; }
        public string HourEnd { get; set; }
        public List<string> WeeksData { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleSubjectDetailsResDto
    {
        public string SubjectName { get; set; }
        public string SubjectTypeColor { get; set; }
        public string SubjectHours { get; set; }
        public string Teachers { get; set; }
        public string TypeAndRoomsName { get; set; }
        public string DepartmentName { get; set; }
        public string SubjectOccur { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ScheduleFilteringData
    {
        public string SelectedYears { get; set; }
        public string WeekInputOptions { get; set; }
    }
}