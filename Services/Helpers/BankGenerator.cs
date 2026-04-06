using HomeBanking.Data.Context;
using HomeBanking.Data.UnitOfWork;
using Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.Helpers
{
    public class BankGenerator
    {
        private static readonly Random _random = new Random();

        // Genera un CBU falso de 22 números
        public static string GenerateCBU()
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, 22)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static async Task<string> GenerateUniqueAliasAsync(IUnitOfWork _unitOfWork)
        {
            string newAlias;
            bool aliasExists;

            do
            {
                newAlias = GenerateAlias();

                // Le pregunto a Azure si ese alias ya existe en la tabla Accounts
                var existingAccount = await _unitOfWork.Accounts.GetAccountByCBUOrAliasAsync(newAlias);
                aliasExists = existingAccount != null;

            } while (aliasExists);

            return newAlias;
        }

        // Genera un Alias tipo "palabra.palabra.palabra"
        private static string GenerateAlias()
        {
            string[] palabras = { "tango", "mate", "rio", "sol", "luna", "pampa", "sur", "norte", "plata", "oro", "azul", "rojo", "madero", "verde", "arbol", "banco", "perro", "casa", "piano", "puerta", "nube", "radio", "camino", "luna", "pasto", "viento", "control", "ventana", "cama", "celeste", "arriba"};
            return $"{palabras[_random.Next(palabras.Length)]}.{palabras[_random.Next(palabras.Length)]}.{palabras[_random.Next(palabras.Length)]}";
        }
    }
}
