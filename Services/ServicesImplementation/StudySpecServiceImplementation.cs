using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class StudySpecServiceImplementation : IStudySpecService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context;

        //--------------------------------------------------------------------------------------------------------------

        public StudySpecServiceImplementation(ServiceHelper helper, IMapper mapper, ApplicationDbContext context)
        {
            _helper = helper;
            _mapper = mapper;
            _context = context;
        }

        //--------------------------------------------------------------------------------------------------------------

        #region Add new study specialization

        /// <summary>
        /// Metoda odpowiedzialna za dodawanie nowych kierunków studiów. Metoda sprawdza, czy kierunek istnieje w
        /// systemie oraz czy powiązany z nim wydział również istnieje. Metoda umożliwa stworzenie jednocześnie dwóch
        /// kierunków studiów (dzienne/zaoczne) jeśli taki parametr znajduje się w obiekcie transferowym.
        /// </summary>
        /// <param name="dto">obiekt transferowy z danymi odnośnie nowego kierunku studiów</param>
        /// <returns>utworzone kierunek/kierunki studiów</returns>
        /// <exception cref="BasicServerException">nieistniejący wydział/duplikat kierunku/brak typu kierunku</exception>
        public async Task<IEnumerable<CreateStudySpecResponseDto>> AddNewStudySpecialization(CreateStudySpecRequestDto dto)
        {
            //wyszukanie wydziału pasującego do kierunku, jeśli nie znajdzie wyrzuci wyjątek
            Department findDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
            if (findDepartment == null) {
                throw new BasicServerException("Nie znaleziono wydziału z podaną nazwą", HttpStatusCode.NotFound);
            }

            List<StudyType> findAllStudyTypes = _context.StudyTypes
                .Where(t => dto.StudyType.Any(id => id == t.Id)).ToList();
            if (findAllStudyTypes.Count == 0) {
                throw new BasicServerException("Nie znaleziono podanych id typów kierunków", HttpStatusCode.NotFound);
            }

            List<StudyDegree> findAllStudyDegrees = _context.StudyDegrees
                .Where(d => dto.StudyDegree.Any(id => id == d.Id)).ToList();
            if (findAllStudyDegrees.Count == 0) {
                throw new BasicServerException("Nie znaleziono podanych id stopni studiów", HttpStatusCode.NotFound);
            }
            
            // przy próbie wprowadzeniu duplikatu kierunku studiów, wyrzuć wyjątek
            StudySpecialization findSpecialization = await _context.StudySpecializations
                .Include(s => s.Department)
                .Include(s => s.StudyType)
                .Include(s => s.StudyDegree)
                .FirstOrDefaultAsync(s => (s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) ||
                                           s.Alias.Equals(dto.Alias, StringComparison.OrdinalIgnoreCase)) &&
                                          s.Department.Name.Equals(dto.DepartmentName, StringComparison.Ordinal) &&
                                          dto.StudyType.Any(v => v == s.StudyType.Id) &&
                                          dto.StudyDegree.Any(v => v == s.StudyDegree.Id));
            
            if (findSpecialization != null) {
                throw new BasicServerException(
                    "Podany kierunek studiów istnieje już w wybranej jednostce.", HttpStatusCode.ExpectationFailed);
            }
            
            List<StudySpecialization> createdSpecializations = new List<StudySpecialization>();

            foreach (StudyType studyType in findAllStudyTypes) {
                foreach (StudyDegree studyDegree in findAllStudyDegrees) {
                    createdSpecializations.Add(new StudySpecialization()
                    {
                        Name = dto.Name,
                        Alias = dto.Alias,
                        DepartmentId = findDepartment.Id,
                        StudyTypeId = studyType.Id,
                        StudyDegreeId = studyDegree.Id,
                    });
                }
            }

            // zapis do bazy danych
            await _context.StudySpecializations.AddRangeAsync(createdSpecializations);
            await _context.SaveChangesAsync();

            return createdSpecializations.Select(s => _mapper.Map<StudySpecResponseDto>(s));
        }

        #endregion
    }
}