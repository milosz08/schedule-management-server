using System.Globalization;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Entity;
using ScheduleManagement.Api.Exception;
using ScheduleManagement.Api.Network.StudySpec;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;
using ScheduleManagement.Api.Util;

namespace ScheduleManagement.Api.Network.ScheduleSubject;

public class ScheduleSubjectServiceImpl(
	ApplicationDbContext dbContext,
	IPasswordHasher<Person> passwordHasher,
	ILogger<ScheduleSubjectServiceImpl> logger,
	IMapper mapper)
	: AbstractExtendedCrudService(dbContext, passwordHasher), IScheduleSubjectService
{
	private readonly TimeSpan _endTime = TimeSpan.ParseExact("22:00", "h\\:mm", new CultureInfo("pl-PL"));
	private readonly TimeSpan _minInterval = TimeSpan.ParseExact("00:05", "h\\:mm", new CultureInfo("pl-PL"));
	private readonly TimeSpan _startTime = TimeSpan.ParseExact("07:00", "h\\:mm", new CultureInfo("pl-PL"));

	public async Task<MessageContentResDto> AddNewScheduleActivity(ScheduleActivityReqDto dto)
	{
		var findStudyGroups = await dbContext.StudyGroups
			.Include(g => g.Department)
			.Include(g => g.StudySpecialization)
			.Where(g => (g.Id == dto.StudyGroupId || dto.IsAddForAllGroups) && g.Department.Id == dto.DeptId &&
			            g.StudySpecialization.Id == dto.StudySpecId)
			.ToListAsync();
		if (findStudyGroups.Count == 0)
		{
			throw new RestApiException("Nie znaleziono grup dziekańskich na podstawie podanych parametrów.",
				HttpStatusCode.NotFound);
		}
		TimeSpan startHour, endHour;
		try
		{
			startHour = TimeSpan.ParseExact(dto.HourStart, "h\\:mm", new CultureInfo("pl-PL"));
			endHour = TimeSpan.ParseExact(dto.HourEnd, "h\\:mm", new CultureInfo("pl-PL"));
			if (startHour >= endHour)
			{
				throw new RestApiException("Godzina rozpoczęcia musi być mniejsza od godziny zakończenia.",
					HttpStatusCode.ExpectationFailed);
			}
			if (startHour < _startTime || endHour > _endTime)
			{
				throw new RestApiException("Nieprawidłowy zakres godzin.", HttpStatusCode.ExpectationFailed);
			}
			var formattedDividedHour = (startHour / _minInterval).ToString(CultureInfo.CurrentCulture);
			if (formattedDividedHour.Contains(NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator))
			{
				throw new RestApiException("Czas odbywania zajęć musi być wielokrotnością pięciu minut.",
					HttpStatusCode.ExpectationFailed);
			}
		}
		catch (FormatException)
		{
			throw new RestApiException("Niepoprawne wartości godzinowe.", HttpStatusCode.ExpectationFailed);
		}
		var findWeekday = await dbContext.Weekdays.FirstOrDefaultAsync(w => w.Id == dto.WeekDayId);

		if (findWeekday == null)
		{
			throw new RestApiException("Nie znaleziono dnia tygodnia na podstawie parametrów.",
				HttpStatusCode.NotFound);
		}
		var findSubjectType = await dbContext.ScheduleSubjectTypes
			.FirstOrDefaultAsync(t => t.Name.Equals(dto.SubjectTypeName, StringComparison.OrdinalIgnoreCase));

		if (findSubjectType == null)
		{
			throw new RestApiException("Nie znaleziono typu przedmiotu na podstawie podanych parametrów.",
				HttpStatusCode.NotFound);
		}
		var findStudySubject = await dbContext.StudySubjects
			.Include(s => s.StudySpecialization)
			.FirstOrDefaultAsync(s => s.StudySpecialization.Id == dto.StudySpecId &&
			                          s.Name.Equals(dto.SubjectOrActivityName, StringComparison.OrdinalIgnoreCase));
		if (findStudySubject == null)
		{
			throw new RestApiException("Nie znaleziono przedmiotu na podstawie podanych parametrów.",
				HttpStatusCode.NotFound);
		}
		var findAllStudyRooms = await dbContext.StudyRooms
			.Include(r => r.Department)
			.Where(r => dto.SubjectRooms.Any(rdto => rdto == r.Id) && r.Department.Id == dto.DeptId)
			.ToListAsync();

		var findAllTeachers = await dbContext.Persons
			.Include(p => p.Role)
			.Include(p => p.Department)
			.Where(p => dto.SubjectTeachers.Any(pdto => pdto == p.Id) && p.Department!.Id == dto.DeptId &&
			            !p.Role.Name.Equals(UserRole.Student))
			.ToListAsync();

		var allScheduleOccurs = new List<WeekScheduleOccur>();
		foreach (var weekData in dto.WeeksData)
		{
			var onlyYearAndWeekNumber = weekData[(weekData.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1)..]
				.Replace(")", "")
				.Split(", ")
				.Select(int.Parse)
				.ToList();
			allScheduleOccurs.Add(new WeekScheduleOccur
			{
				WeekIdentifier = (byte)onlyYearAndWeekNumber[1],
				Year = onlyYearAndWeekNumber[0],
				OccurDate = DateUtils.FindDayBasedDayIdAndWeekNumber(onlyYearAndWeekNumber[0],
					onlyYearAndWeekNumber[1], findWeekday.Identifier)
			});
		}
		var allOccursConvert = allScheduleOccurs.Select(o => $"{o.Year},{o.WeekIdentifier}").ToList();
		var findDuplicatRooms = await dbContext.ScheduleSubjects
			.Include(sb => sb.Weekday)
			.Include(sb => sb.StudyRooms)
			.Include(sb => sb.StudyGroups)
			.Include(sb => sb.WeekScheduleOccurs)
			.ToListAsync();

		foreach (var sb in findDuplicatRooms)
		{
			if (sb.StudyGroups.Any(sgb => findStudyGroups.Any(sg => sgb.Id == sg.Id)))
			{
				var convertOccured = sb.WeekScheduleOccurs.Select(o => $"{o.Year},{o.WeekIdentifier}").ToList();
				var hours = sb.StartTime < TimeSpan.Parse(dto.HourStart)
				            || TimeSpan.Parse(dto.HourStart) < sb.EndTime
				            || sb.StartTime < TimeSpan.Parse(dto.HourEnd)
				            || TimeSpan.Parse(dto.HourEnd) < sb.EndTime;

				if (sb.Weekday.Id == dto.WeekDayId && sb.StudyYear.Equals(dto.StudyYear) && hours &&
				    (convertOccured.Intersect(allOccursConvert).Any()
				     || (convertOccured.IsNullOrEmpty() && allOccursConvert.IsNullOrEmpty())))
				{
					throw new RestApiException(
						"Termin ma już przypisaną aktywność. Proszę dodać aktywność dla innego terminu lub " +
						"usunąć kolizję.", HttpStatusCode.Forbidden);
				}
			}
		}
		var addingScheduleSubject = new Entity.ScheduleSubject
		{
			ScheduleSubjectTypeId = findSubjectType.Id,
			WeekScheduleOccurs = allScheduleOccurs,
			StudySubjectId = findStudySubject.Id,
			ScheduleTeachers = findAllTeachers,
			StudyRooms = findAllStudyRooms,
			StudyGroups = findStudyGroups,
			WeekdayId = findWeekday.Id,
			StudyYear = dto.StudyYear,
			StartTime = startHour,
			EndTime = endHour
		};
		await dbContext.ScheduleSubjects.AddAsync(addingScheduleSubject);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully add new schedule activity: {}", addingScheduleSubject);
		return new MessageContentResDto
		{
			Message = "Pomyślnie dodano nową aktywność."
		};
	}

	public async Task<ScheduleDataRes<ScheduleGroups>> GetAllScheduleSubjectsBaseGroup(ScheduleGroupQuery dto,
		ScheduleFilteringData filter)
	{
		var findStudyGroup = await dbContext.StudyGroups
			.Include(g => g.Semester)
			.Include(g => g.Department)
			.Include(g => g.StudySpecialization)
			.Include(g => g.StudySpecialization.StudyType)
			.Include(g => g.StudySpecialization.StudyDegree)
			.FirstOrDefaultAsync(g => g.Id == dto.GroupId && g.Department.Id == dto.DeptId &&
			                          g.StudySpecialization.Id == dto.SpecId);
		if (findStudyGroup == null)
		{
			throw new RestApiException("Błędne parametry planu.", HttpStatusCode.NotFound);
		}
		var response = new ScheduleDataRes<ScheduleGroups>
		{
			TraceDetails =
			[
				"Grupy",
				findStudyGroup.Department.Name,
				findStudyGroup.StudySpecialization.StudyDegree.Name,
				findStudyGroup.StudySpecialization.StudyType.Name,
				findStudyGroup.StudySpecialization.Name,
				findStudyGroup.Semester.Name
			],
			ScheduleHeaderData = $"Plan zajęć - {findStudyGroup.Name}, rok {filter.SelectedYears}"
		};
		var allWeekdays = await dbContext.Weekdays
			.Where(d => findStudyGroup.StudySpecialization.StudyType.Alias.Equals(StudySpecType.St)
				? d.Identifier > 0 && d.Identifier < 6
				: d.Identifier > 4 && d.Identifier < 8)
			.Select(d => d)
			.ToListAsync();

		var dayIncrement = findStudyGroup.StudySpecialization.StudyType.Alias == StudySpecType.St ? 0 : 4;
		foreach (var weekday in allWeekdays)
		{
			var singleDay = new ScheduleCanvasData<ScheduleGroups>();
			var findAllScheduleSubjects = await dbContext.ScheduleSubjects
				.Include(s => s.Weekday)
				.Include(s => s.StudyRooms).ThenInclude(studyRoom => studyRoom.Department)
				.Include(scheduleSubject => scheduleSubject.StudyRooms)
				.ThenInclude(studyRoom => studyRoom.Cathedral)
				.Include(s => s.StudySubject)
				.Include(s => s.WeekScheduleOccurs)
				.Include(s => s.ScheduleSubjectType)
				.Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Cathedral)
				.Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Department)
				.Where(s => s.Weekday.Id == weekday.Id && s.StudyGroups.Any(stg => stg.Id == dto.GroupId))
				.ToListAsync();

			var allScheduleSubjectsBaseGroup = new List<ScheduleGroups>();

			foreach (var scheduleSubject in findAllScheduleSubjects)
			{
				var baseData = ScheduleSubjectFilledFieldData(scheduleSubject, filter);
				var scheduleGroups = mapper.Map<ScheduleGroups>(baseData);

				scheduleGroups.TeachersAliases = MappingScheduleTeachers(scheduleSubject);
				scheduleGroups.RoomsAliases = MappingStudyRooms(scheduleSubject);

				FilteringScheduleSubject(scheduleSubject, filter, ref allScheduleSubjectsBaseGroup,
					ref scheduleGroups);
			}
			AddBaseValuesForSingleDay(ref singleDay, weekday, filter, ref dayIncrement);
			singleDay.WeekdayData = allScheduleSubjectsBaseGroup;

			response.ScheduleCanvasData.Add(singleDay);
		}
		response.CurrentChooseWeek = filter.WeekInputOptions;
		return response;
	}

	public async Task<ScheduleDataRes<ScheduleTeachers>> GetAllScheduleSubjectsBaseTeacher(ScheduleTeacherQuery dto,
		ScheduleFilteringData filter)
	{
		var findTeacher = await dbContext.Persons
			.Include(t => t.Role)
			.Include(t => t.Department)
			.Include(t => t.Cathedral)
			.Where(t => !t.Role.Name.Equals(UserRole.Administrator))
			.FirstOrDefaultAsync(t =>
				t.Id == dto.EmployeerId && t.Department!.Id == dto.DeptId && t.Cathedral!.Id == dto.CathId);

		if (findTeacher == null)
		{
			throw new RestApiException("Błędne parametry planu.", HttpStatusCode.NotFound);
		}
		var response = new ScheduleDataRes<ScheduleTeachers>
		{
			TraceDetails =
			[
				"Pracownicy",
				findTeacher.Department!.Name,
				findTeacher.Cathedral!.Name
			],
			ScheduleHeaderData = $"Plan zajęć - {findTeacher.Surname} {findTeacher.Name}, rok {filter.SelectedYears}"
		};
		var dayIncrement = 0;
		foreach (var weekday in await dbContext.Weekdays.Select(d => d).ToListAsync())
		{
			var singleDay = new ScheduleCanvasData<ScheduleTeachers>();

			var findAllScheduleTeachers = await dbContext.ScheduleSubjects
				.Include(s => s.Weekday)
				.Include(s => s.StudyRooms).ThenInclude(str => str.Department)
				.Include(s => s.StudyGroups)
				.Include(s => s.StudySubject)
				.Include(s => s.WeekScheduleOccurs)
				.Include(s => s.ScheduleSubjectType)
				.Include(s => s.StudyRooms).ThenInclude(ste => ste.Cathedral)
				.Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Department)
				.Include(s => s.StudyGroups).ThenInclude(ste => ste.StudySpecialization)
				.Where(s => s.Weekday.Id == weekday.Id && s.ScheduleTeachers.Any(st => st.Id == dto.EmployeerId))
				.ToListAsync();

			var allScheduleSubjectsBaseTeacher = new List<ScheduleTeachers>();
			foreach (var scheduleSubject in findAllScheduleTeachers)
			{
				var baseData = ScheduleSubjectFilledFieldData(scheduleSubject, filter);
				var scheduleTeachers = mapper.Map<ScheduleTeachers>(baseData);

				scheduleTeachers.StudyGroupAliases = MappingScheduleGroups(scheduleSubject);
				scheduleTeachers.RoomsAliases = MappingStudyRooms(scheduleSubject);

				FilteringScheduleSubject(scheduleSubject, filter, ref allScheduleSubjectsBaseTeacher,
					ref scheduleTeachers);
			}
			AddBaseValuesForSingleDay(ref singleDay, weekday, filter, ref dayIncrement);
			singleDay.WeekdayData = allScheduleSubjectsBaseTeacher;

			response.ScheduleCanvasData.Add(singleDay);
		}
		response.CurrentChooseWeek = filter.WeekInputOptions;
		return response;
	}

	public async Task<ScheduleDataRes<ScheduleRooms>> GetAllScheduleSubjectsBaseRoom(ScheduleRoomQuery dto,
		ScheduleFilteringData filter)
	{
		var findStudyRoom = await dbContext.StudyRooms
			.Include(r => r.RoomType)
			.Include(r => r.Department)
			.Include(r => r.Cathedral)
			.FirstOrDefaultAsync(r =>
				r.Id == dto.RoomId && r.Department.Id == dto.DeptId && r.Cathedral.Id == dto.CathId);

		if (findStudyRoom == null)
		{
			throw new RestApiException("Błędne parametry planu.", HttpStatusCode.NotFound);
		}
		var response = new ScheduleDataRes<ScheduleRooms>
		{
			TraceDetails =
			[
				"Sale zajęciowe",
				findStudyRoom.Department.Name,
				findStudyRoom.Cathedral.Name
			]
		};
		var ifRoomDescription = string.Empty.Equals(findStudyRoom.Description) ? $"({findStudyRoom.Description})" : "";
		response.ScheduleHeaderData =
			$"Plan zajęć - {findStudyRoom.Name} {ifRoomDescription}, rok {filter.SelectedYears}";

		var dayIncrement = 0;
		foreach (var weekday in await dbContext.Weekdays.Select(d => d).ToListAsync())
		{
			var singleDay = new ScheduleCanvasData<ScheduleRooms>();
			var findAllScheduleStudyRooms = await dbContext.ScheduleSubjects
				.Include(s => s.Weekday)
				.Include(s => s.StudyGroups)
				.Include(s => s.StudySubject)
				.Include(s => s.ScheduleTeachers)
				.Include(s => s.WeekScheduleOccurs)
				.Include(s => s.ScheduleSubjectType)
				.Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Cathedral)
				.Include(s => s.ScheduleTeachers).ThenInclude(ste => ste.Department)
				.Include(s => s.StudyGroups).ThenInclude(ste => ste.StudySpecialization)
				.Where(s => s.Weekday.Id == weekday.Id && s.StudyRooms.Any(st => st.Id == dto.RoomId))
				.ToListAsync();

			var allScheduleSubjectsBaseRoom = new List<ScheduleRooms>();

			foreach (var scheduleSubject in findAllScheduleStudyRooms)
			{
				var baseData = ScheduleSubjectFilledFieldData(scheduleSubject, filter);
				var scheduleRooms = mapper.Map<ScheduleRooms>(baseData);

				scheduleRooms.StudyGroupAliases = MappingScheduleGroups(scheduleSubject);
				scheduleRooms.TeachersAliases = MappingScheduleTeachers(scheduleSubject);

				FilteringScheduleSubject(scheduleSubject, filter, ref allScheduleSubjectsBaseRoom,
					ref scheduleRooms);
			}
			AddBaseValuesForSingleDay(ref singleDay, weekday, filter, ref dayIncrement);
			singleDay.WeekdayData = allScheduleSubjectsBaseRoom;

			response.ScheduleCanvasData.Add(singleDay);
		}
		response.CurrentChooseWeek = filter.WeekInputOptions;
		return response;
	}

	public async Task<ScheduleSubjectDetailsResDto> GetScheduleSubjectDetails(long schedSubjId)
	{
		var findSubject = await dbContext.ScheduleSubjects
			.Include(s => s.StudyRooms)
			.Include(s => s.StudySubject)
			.Include(s => s.ScheduleTeachers)
			.Include(s => s.WeekScheduleOccurs)
			.Include(s => s.ScheduleSubjectType)
			.Include(s => s.StudySubject.Department)
			.FirstOrDefaultAsync(s => s.Id == schedSubjId);

		if (findSubject == null)
		{
			throw new RestApiException("Nie znaleziono przedmiotu z planu zajęć z podanym ID",
				HttpStatusCode.NotFound);
		}
		return new ScheduleSubjectDetailsResDto
		{
			SubjectName = $"{findSubject.StudySubject.Name}",
			SubjectTypeColor = findSubject.ScheduleSubjectType.Color,
			SubjectHours = DateUtils.FormatTime(findSubject),
			Teachers = string.Join(", ", findSubject.ScheduleTeachers.Select(t => $"{t.Name} {t.Surname}")),
			TypeAndRoomsName = $"{findSubject.ScheduleSubjectType.Name}, " +
			                   $"sala: {string.Join(", ", findSubject.StudyRooms.Select(r => r.Name))}",
			DepartmentName = $"{findSubject.StudySubject.Department.Name}, " +
			                 $"({findSubject.StudySubject.Department.Alias})",
			SubjectOccur = DateUtils.ConvertScheduleOccur(findSubject)
		};
	}

	protected override async Task<MessageContentResDto> OnDeleteSelected(DeleteSelectedRequestDto items,
		UserCredentialsHeaderDto userCredentialsHeader)
	{
		var findScheduleSubject = await dbContext.ScheduleSubjects
			.Include(s => s.StudySubject).ThenInclude(sb => sb.Department)
			.FirstOrDefaultAsync(d => items.ElementsIds.Any(sb => sb == d.Id));

		if (findScheduleSubject == null)
		{
			throw new RestApiException("Nie znaleziono szukanego przedmiotu.", HttpStatusCode.NotFound);
		}
		if (findScheduleSubject.StudySubject.Department.Id != userCredentialsHeader.Person.Department!.Id &&
		    UserRole.Editor.Equals(userCredentialsHeader.Person.Role.Name) &&
		    !UserRole.IsAdministrator(userCredentialsHeader.Person))
		{
			throw new RestApiException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora " +
			                           "lub próba usunięcia chronionego zasobu z rangą edytora.",
				HttpStatusCode.Forbidden);
		}
		var message = "Nie usunięto żadnego przedmiotu z planu.";
		var toRemoved = dbContext.ScheduleSubjects.Where(s => items.ElementsIds.Any(id => id == s.Id));
		if (toRemoved.Any())
		{
			message = $"Pomyślnie usunięto wybrane przedmioty z planu. " +
			          $"Liczba usuniętych przedmiotów: {toRemoved.Count()}.";
		}
		dbContext.ScheduleSubjects.RemoveRange(toRemoved);
		await dbContext.SaveChangesAsync();

		logger.LogInformation("Successfully removed: {} study subjects from schedule", toRemoved.Count());
		return new MessageContentResDto
		{
			Message = message
		};
	}

	protected override Task<MessageContentResDto> OnDeleteAll(UserCredentialsHeaderDto userCredentialsHeaderDto)
	{
		return Task.FromResult(new MessageContentResDto
		{
			Message = "Funkcjonalność wyłączona."
		});
	}

	private static void FilteringScheduleSubject<T>(Entity.ScheduleSubject scheduleSubject,
		ScheduleFilteringData filter, ref List<T> allElements, ref T element)
	{
		var isShowing = false;
		if (!filter.WeekInputOptions.Equals("pokaż wszystko", StringComparison.OrdinalIgnoreCase))
		{
			var options = filter
				.WeekInputOptions[(filter.WeekInputOptions.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1)..]
				.Replace(")", "").Split(", ").Select(int.Parse).ToList();
			isShowing = scheduleSubject.WeekScheduleOccurs.Any(o =>
				            o.Year == options[0] && o.WeekIdentifier == options[1])
			            || scheduleSubject.WeekScheduleOccurs.IsNullOrEmpty();
		}
		else if (scheduleSubject.StudyYear.Equals(filter.SelectedYears))
		{
			isShowing = true;
		}
		if (isShowing)
		{
			allElements.Add(element);
		}
	}

	private static BaseScheduleResData ScheduleSubjectFilledFieldData(Entity.ScheduleSubject subj,
		ScheduleFilteringData filter)
	{
		return new BaseScheduleResData
		{
			ScheduleSubjectId = subj.Id,
			SubjectWithTypeAlias = StringUtils.CreateSubjectAlias(subj),
			SubjectTypeHexColor = subj.ScheduleSubjectType.Color,
			SubjectTime = DateUtils.FormatTime(subj),
			PositionFromTop = DateUtils.ComputedPositionFromTopAndHeight(subj.StartTime, subj.EndTime).pxFromTop,
			ElementHeight = DateUtils.ComputedPositionFromTopAndHeight(subj.StartTime, subj.EndTime).pxHegith,
			SubjectOccuredData = DateUtils.ConvertScheduleOccur(subj),
			IfNotShowingOccuredDates =
				!filter.WeekInputOptions.Equals("pokaż wszystko", StringComparison.OrdinalIgnoreCase)
		};
	}

	private static List<ScheduleMultipleValues<ScheduleGroupQuery>> MappingScheduleGroups(
		Entity.ScheduleSubject scheduleSubject)
	{
		return scheduleSubject.StudyGroups
			.Select(g => new ScheduleMultipleValues<ScheduleGroupQuery>(
				g.Name,
				new ScheduleGroupQuery(g.Department.Id, g.StudySpecialization.Id, g.Id))
			).ToList();
	}

	private static List<ScheduleMultipleValues<ScheduleTeacherQuery>> MappingScheduleTeachers(
		Entity.ScheduleSubject scheduleSubject)
	{
		return scheduleSubject.ScheduleTeachers
			.Select(t => new ScheduleMultipleValues<ScheduleTeacherQuery>(
				t.Shortcut,
				new ScheduleTeacherQuery(t.Department!.Id, t.Cathedral!.Id, t.Id))
			).ToList();
	}


	private static List<ScheduleMultipleValues<ScheduleRoomQuery>> MappingStudyRooms(
		Entity.ScheduleSubject scheduleSubject)
	{
		return scheduleSubject.StudyRooms
			.Select(r => new ScheduleMultipleValues<ScheduleRoomQuery>(
				r.Name,
				new ScheduleRoomQuery(r.Department.Id, r.Cathedral.Id, r.Id))
			).ToList();
	}

	private static void AddBaseValuesForSingleDay<T>(ref ScheduleCanvasData<T> canvasData, Weekday weekday,
		ScheduleFilteringData filter, ref int dayIncrement)
	{
		canvasData.WeekdayNameWithId = new NameIdElementDto(weekday.Id, weekday.Alias);
		canvasData.IfNotShowingOccuredDates = filter.WeekInputOptions
			.Equals("pokaż wszystko", StringComparison.OrdinalIgnoreCase);
		if (!filter.WeekInputOptions.Equals("pokaż wszystko", StringComparison.OrdinalIgnoreCase))
		{
			var options = filter
				.WeekInputOptions[(filter.WeekInputOptions.IndexOf("(", StringComparison.OrdinalIgnoreCase) + 1)..]
				.Replace(")", "").Split(", ").Select(int.Parse).ToList();
			canvasData.WeekdayDateTime = DateUtils.FirstDateOfWeekBasedWeekNumber(options[0], options[1])
				.AddDays(dayIncrement++).ToString("dd\\.MM");
		}
		else
		{
			canvasData.WeekdayDateTime = filter.WeekInputOptions;
		}
	}
}
