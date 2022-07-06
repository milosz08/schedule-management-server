/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: FilesServiceImplementation.cs
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

using System;

using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class FilesServiceImplementation : IFilesService
    {
        private readonly ApplicationDbContext _context;
        
        private readonly static string[] ACCEPTABLE_IMAGE_TYPES = { "image/jpeg" };
        private readonly string FOLDER_PATH = $"{ROOT_PATH}/_StaticPrivateContent/UserImages";
        
        private readonly static string ROOT_PATH = Directory.GetCurrentDirectory();

        //--------------------------------------------------------------------------------------------------------------
        
        public FilesServiceImplementation(ApplicationDbContext context)
        {
            _context = context;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda odpowiedzialna za pobieranie osoby z bazy danych.
        /// </summary>
        /// <param name="userLogin">login użytkownika</param>
        /// <returns>znaleziona osoba w postaci obiektu</returns>
        /// <exception cref="BasicServerException">w przypadku nieznalezienia osoby</exception>
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

        /// <summary>
        /// Metoda zwraca obraz w postaci tupli reprezentującej tablicę bajtów (zdjęcie) oraz rozszerzenie.
        /// </summary>
        /// <param name="userId">id użytkownika</param>
        /// <param name="userLogin">login użytkownika</param>
        /// <returns>tupla w postaci tablicy bajtów (dane) oraz typu pliku (image/jpeg)</returns>
        /// <exception cref="BasicServerException">dostęp do nieistniejącego lub zabronionego zasobu</exception>
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

        /// <summary>
        /// Metoda odpowiada za zmianę avataru użytkownika (na podstawie claimu w tokenie JWT).
        /// </summary>
        /// <param name="image">obrazek</param>
        /// <param name="userLogin">login użytkownika (wartość typu Claim)</param>
        /// <returns>obiekt z wiadomością serwera</returns>
        /// <exception cref="BasicServerException">brak zasobu lub problem z dodaniem zasobu</exception>
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
            if (!Directory.Exists(FOLDER_PATH)) { // jeśli folder nie istnieje, stwórz
                Directory.CreateDirectory(FOLDER_PATH);
            }
            string fullPath = $"{FOLDER_PATH}/{findPerson.DictionaryHash}__{findPerson.Login}" +
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

        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete custom avatar
        
        /// <summary>
        /// Metoda odpowiadająca za usuwanie zdjęcia użytkownika. Tożsamość sprawdzana jest na podstawie JWT oraz
        /// odpowiadających im wartości claimów.
        /// </summary>
        /// <param name="userLogin">login użytkownika (wartość typu Claim)</param>
        /// <returns>obiekt z wiadomością serwera</returns>
        public async Task<PseudoNoContentResponseDto> UserRemoveCustomAvatar(Claim userLogin)
        {
            // znajdowanie osoby na podstawie tokenu JWT
            Person findPerson = await GetPersonFromDb(userLogin);
            string fullPath = $"{FOLDER_PATH}/{findPerson.DictionaryHash}__{findPerson.Login}" +
                              $"{new FileInfo(ACCEPTABLE_IMAGE_TYPES[0]).Extension}";
            
            // procedura usuwania zasobu z serwera
            File.Delete($"{fullPath}.jpg");
            
            findPerson.HasPicture = false;
            await _context.SaveChangesAsync();
            
            return new PseudoNoContentResponseDto()
            {
                Message = $"Zdjęcie profilowe użytkownika {findPerson.Name} {findPerson.Surname} zostało usunięte z serwera.",
            };
        }

        #endregion
    }
}