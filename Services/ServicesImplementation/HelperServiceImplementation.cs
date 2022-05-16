using AutoMapper;

using System.Net;
using System.Linq;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class HelperServiceImplementation : IHelperService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public HelperServiceImplementation(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        //--------------------------------------------------------------------------------------------------------------

        #region Get available study types

        /// <summary>
        /// Metoda pobierająca wszystkie typy studiów z tabeli i zwracająca poprzez obiekt DTO przechowujący dane
        /// w formie tabeli.
        /// </summary>
        /// <returns>znalezione typy studiów</returns>
        /// <exception cref="BasicServerException">jeśli tableca z typami studiów jest pusta</exception>
        public AvailableStudyTypesResponseDto GetAvailableStudyTypes()
        {
            // jeśli nie znajdzie żadnych elementów w tablicy, wyrzuć wyjątek
            if (!_context.StudyTypes.Any()) {
                throw new BasicServerException("Nie znaleziono typów studiów", HttpStatusCode.NotFound);
            }
            
            // wypłaszczanie wyniku i mapowanie na obiekt transferowy (DTO)
            List<StudySingleTypeDto> mappedStudyType = _context.StudyTypes
                .Select(t => _mapper.Map<StudySingleTypeDto>(t)).ToList();
            
            return new AvailableStudyTypesResponseDto()
            {
                StudyTypes = mappedStudyType,
            };
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get available pagination types

        /// <summary>
        /// Metoda pobierająca zamockowane dane służące do paginacji rezultatów na front-endzie.
        /// </summary>
        /// <returns>dostępne paginacje stron wyszukiwarki</returns>
        public AvailablePaginationSizes GetAvailablePaginationTypes()
        {
            return new AvailablePaginationSizes()
            {
                AvailablePaginations = ApplicationUtils._allowedPageSizes.ToList(),
            };
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Get available study room types
        
        /// <summary>
        /// Metoda pobierająca wszystkie elementy z tabeli opisującej typy sal zajęciowych i zwracających w formie
        /// zmapowanej tablicy.
        /// </summary>
        /// <returns>wszystkie dostępne typy sal zajęciowych</returns>
        /// <exception cref="BasicServerException">gdy w tabeli nie znajdują się żadne sale zajęciowe</exception>
        public AvailableRoomTypesResponseDto GetAvailableRoomTypes()
        {
            // jeśli nie znajdzie żadnych elementów w tablicy, zwróć wyjątek
            if (!_context.RoomTypes.Any()) {
                throw new BasicServerException("Nie znaleziono dostępnych typów sal zajęciowych",
                    HttpStatusCode.NotFound);
            }

            List<SingleRoomTypeDto> mappedRoomType =
                _context.RoomTypes.Select(r => _mapper.Map<SingleRoomTypeDto>(r)).ToList();

            return new AvailableRoomTypesResponseDto()
            {
                StudyRoomTypes = mappedRoomType,
            };
        }

        #endregion
    }
}