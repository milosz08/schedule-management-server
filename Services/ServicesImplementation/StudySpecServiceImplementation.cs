using AutoMapper;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class StudySpecServiceImplementation : IStudySpecService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        
        //--------------------------------------------------------------------------------------------------------------

        public StudySpecServiceImplementation(IMapper mapper, ApplicationDbContext context)
        {
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
        public async Task<IEnumerable<StudySpecResponseDto>> AddNewStudySpecialization(StudySpecRequestDto dto)
        {
            //wyszukanie wydziału pasującego do kierunku, jeśli nie znajdzie wyrzuci wyjątek
            Department findDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.ToLower() == dto.DepartmentName.ToLower());
            if (findDepartment == null) {
                throw new BasicServerException("Nie znaleziono wydziału z podaną nazwą", HttpStatusCode.NotFound);
            }
            
            // przy próbie wprowadzeniu duplikatu kierunku studiów, wyrzuć wyjątek
            StudySpecialization findSpecialization = await _context.StudySpecializations
                .Include(s => s.Department)
                .Include(s => s.StudyType)
                .FirstOrDefaultAsync(s => (s.Name.ToLower() == dto.Name.ToLower() 
                                           || s.Alias.ToLower() == dto.Alias.ToLower()) 
                                          && s.Department.Name.ToLower() == dto.DepartmentName.ToLower()
                                          && s.StudyType.Alias.ToLower() == dto.StudyType.ToLower());
            if (findSpecialization != null) {
                throw new BasicServerException(
                    "Podany kierunek studiów istnieje już w wybranej jednostce.", HttpStatusCode.ExpectationFailed);
            }
            
            List<StudySpecialization> createdSpecializations = new List<StudySpecialization>();
            
            if (dto.StudyType == StudySpecTypes.ALL) {
                List<long> allStudyTypes = _context.StudyTypes.Select(t => t.Id).ToList();
                if (allStudyTypes.Count == 0) {
                    throw new BasicServerException("Nie znaleziono żadnych typów kierunków.", HttpStatusCode.NotFound);
                }
                foreach (long typeId in allStudyTypes) {
                    createdSpecializations.Add(new StudySpecialization()
                    {
                        Name = dto.Name,
                        Alias = dto.Alias,
                        DepartmentId = findDepartment.Id,
                        StudyTypeId = typeId,
                    });
                }
            } else {
                StudyType studyType = await _context.StudyTypes.FirstOrDefaultAsync(t => t.Alias == dto.StudyType);
                if (studyType == null) {
                    throw new BasicServerException("Nie znaleziono podanego typu kierunku.", HttpStatusCode.NotFound);
                }
                createdSpecializations.Add(new StudySpecialization()
                {
                    Name = dto.Name,
                    Alias = dto.Alias,
                    DepartmentId = findDepartment.Id,
                    StudyTypeId = studyType.Id,
                });
            }

            // zapis do bazy danych
            await _context.StudySpecializations.AddRangeAsync(createdSpecializations);
            await _context.SaveChangesAsync();

            return createdSpecializations.Select(s => _mapper.Map<StudySpecResponseDto>(s));
        }

        #endregion
    }
}