using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.DbConfig;

using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class FilesServiceImplementation : IFilesService
    {
        private readonly ApplicationDbContext _context;
        
        private readonly static string[] ACCEPTABLE_IMAGE_TYPES = { "image/jpeg" };
        private readonly static string ROOT_PATH = Directory.GetCurrentDirectory();

        //--------------------------------------------------------------------------------------------------------------
        
        public FilesServiceImplementation(ApplicationDbContext context)
        {
            _context = context;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        private async Task<Person> GetPersonFromDb(Claim userLogin)
        {
            // jeśli JWT jest nieprawidłowy, rzuć wyjątek dostępu (forbidden 403)
            if (userLogin == null) {
                throw new BasicServerException("Dostęp do zasobu zabroniony.", HttpStatusCode.Forbidden);
            }
            // wyszukaj osobę na podstawie loginu z JWT, jeśli nie znajdzie rzuć wyjątek 404
            Person findPerson = await _context.Persons.FirstOrDefaultAsync(p => p.Login == userLogin.Value);
            if (findPerson == null) {
                throw new BasicServerException("Podany użytkownik nie istenieje w systemie.", HttpStatusCode.NotFound);
            }
            return findPerson;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Get Custom Avatar

        // metoda zwraca obraz w postaci tupli reprezentującej tablicę bajtów (zdjęcie) oraz rozszerzenie
        public async Task<(byte[], string)> UserGetCustomAvatar(string userId, Claim userLogin)
        {
            Person findPerson = await GetPersonFromDb(userLogin);
            // jeśli próba odczytania awataru innego niż własnego rzuć wyjątek 403 forbidden
            if (userId != findPerson.DictionaryHash) {
                throw new BasicServerException(
                    "Dostęp do nieistniejącego lub zabronionego zasobu", HttpStatusCode.Forbidden);
            }
            
            string filePath = $"{ROOT_PATH}/_StaticPrivateContent/UserImages";
            string fileName = $"{filePath}/{findPerson.DictionaryHash}__{findPerson.Login}.jpg";
            
            byte[] file = File.ReadAllBytes(fileName);
            return (file, "image/jpeg");
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Add Custom Avatar

        // metoda odpowiada za zmianę avataru użytkownika (na podstawie claimu w tokenie JWT)
        public async Task<PseudoNoContentResponseDto> UserAddCustomAvatar(IFormFile image, Claim userLogin)
        {
            Person findPerson = await GetPersonFromDb(userLogin);
            // jeśli przesłana grafika jest uszkodzona lub nie istnieje (malformed data exception)
            if (image == null || image.Length == 0) {
                throw new BasicServerException(
                    "Błąd podczas dodawania obrazu. Spróbuj ponownie.", HttpStatusCode.ExpectationFailed);
            }
            // jeśli plik jest w niezgodnym formacie rzuć wyjątek 417
            if (Array.IndexOf(ACCEPTABLE_IMAGE_TYPES, image.ContentType) == -1) {
                throw new BasicServerException(
                    "Akceptowane rozszerzenia pliku to: .jpeg", HttpStatusCode.ExpectationFailed);
            }
            string folderPath = $"{ROOT_PATH}/_StaticPrivateContent/UserImages";
            if (!Directory.Exists(folderPath)) { // jeśli folder nie istnieje, stwórz
                Directory.CreateDirectory(folderPath);
            }
            string fullPath = $"{folderPath}/{findPerson.DictionaryHash}__{findPerson.Login}" +
                              $"{new FileInfo(image.FileName).Extension}";
            
            FileStream stream = new FileStream(fullPath, FileMode.Create);
            image.CopyTo(stream);
            
            findPerson.HasPicture = true;
            await _context.SaveChangesAsync();

            stream.Close();
            return new PseudoNoContentResponseDto()
            {
                Message = $"Zdjęcie profilowe użytkownika {findPerson.Name} {findPerson.Surname} zostało ustawione.",
            };
        }

        #endregion
    }
}