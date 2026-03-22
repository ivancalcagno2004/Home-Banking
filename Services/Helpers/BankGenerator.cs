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

        // Genera un Alias tipo "palabra.palabra.palabra"
        public static string GenerateAlias()
        {
            string[] palabras = { "tango", "mate", "rio", "sol", "luna", "pampa", "sur", "norte", "plata", "oro", "azul", "rojo", "madero", "verde", "arbol", "banco", "perro", "casa", "piano", "puerta", "nube", "radio", "camino", "luna", "pasto", "viento"};
            return $"{palabras[_random.Next(palabras.Length)]}.{palabras[_random.Next(palabras.Length)]}.{palabras[_random.Next(palabras.Length)]}";
        }
    }
}
