using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encryption_algorithms
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine("Algoritmo de encriptación basado en año de nacimiento");
            Console.WriteLine("----------------------------------------------------");

            // Flujo principal: pedir año, encriptar, mostrar resultado, opción desencriptar
            int birthYear = ObtenerAñoNacimiento();
            bool continuar = true;

            while (continuar)
            {
                // Paso 1: Pedir palabra o texto a encriptar
                Console.Write("\nIngrese la palabra o texto a encriptar: ");
                string textoOriginal = Console.ReadLine();

                if (string.IsNullOrEmpty(textoOriginal))
                {
                    Console.WriteLine("Debe ingresar un texto para encriptar.");
                    continue;
                }

                // Paso 2: Encriptar y mostrar el resultado
                string mensajeEncriptado = Encrypt(textoOriginal, birthYear);
                Console.WriteLine("\nMensaje encriptado:");
                Console.WriteLine(mensajeEncriptado);

                // Paso 3: Opción de desencriptar
                Console.Write("\n¿Desea desencriptar este mensaje? (S/N): ");
                string respuesta = Console.ReadLine().Trim().ToUpper();

                if (respuesta == "S")
                {
                    // Desencriptar y mostrar el resultado
                    string mensajeDesencriptado = Decrypt(mensajeEncriptado, birthYear);
                    Console.WriteLine("\nMensaje desencriptado:");
                    Console.WriteLine(mensajeDesencriptado);
                }

                // Opciones adicionales
                Console.WriteLine("\nSeleccione una opción:");
                Console.WriteLine("1. Encriptar otra palabra (mismo año de nacimiento)");
                Console.WriteLine("2. Cambiar año de nacimiento y encriptar otra palabra");
                Console.WriteLine("3. Salir");
                Console.Write("Opción: ");

                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        // Continuar al siguiente ciclo con el mismo año
                        break;
                    case "2":
                        // Pedir nuevo año de nacimiento
                        birthYear = ObtenerAñoNacimiento();
                        break;
                    case "3":
                        continuar = false;
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Continuando con el mismo año de nacimiento.");
                        break;
                }
            }

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }

        /// <summary>
        /// Solicita y valida el año de nacimiento
        /// </summary>
        static int ObtenerAñoNacimiento()
        {
            int birthYear;

            while (true)
            {
                Console.Write("Ingrese su año de nacimiento: ");
                if (int.TryParse(Console.ReadLine(), out birthYear))
                {
                    // Validación simple para asegurar que el año sea razonable
                    if (birthYear > 0 && birthYear < 3000)
                    {
                        return birthYear;
                    }
                }

                Console.WriteLine("Año inválido. Por favor ingrese un número válido.");
            }
        }

        /// <summary>
        /// Encripta un mensaje usando el año de nacimiento
        /// </summary>
        /// <param name="message">El mensaje a encriptar</param>
        /// <param name="birthYear">El año de nacimiento para la encriptación</param>
        /// <returns>Valores encriptados separados por espacios</returns>
        static string Encrypt(string message, int birthYear)
        {
            StringBuilder encrypted = new StringBuilder();

            foreach (char c in message)
            {
                int unicodeValue = (int)c;

                // Para caracteres con valores Unicode altos (como chino, japonés, etc.)
                // utilizamos una operación diferente para mantener valores positivos
                int encryptedValue;

                if (unicodeValue > birthYear)
                {
                    // Para caracteres con valores Unicode mayores que el año de nacimiento
                    // agregamos un indicador (-1) seguido del valor Unicode
                    encryptedValue = -1; // Indicador especial

                    // Agregar un espacio entre números si no es el primer carácter
                    if (encrypted.Length > 0)
                    {
                        encrypted.Append(" ");
                    }

                    encrypted.Append(encryptedValue);

                    // Agregar el valor Unicode directamente
                    encrypted.Append(" ");
                    encrypted.Append(unicodeValue);
                }
                else
                {
                    // Para caracteres normales (como letras latinas), usar la operación original
                    encryptedValue = birthYear - unicodeValue;

                    // Agregar un espacio entre números si no es el primer carácter
                    if (encrypted.Length > 0)
                    {
                        encrypted.Append(" ");
                    }

                    encrypted.Append(encryptedValue);
                }
            }

            return encrypted.ToString();
        }

        /// <summary>
        /// Desencripta un mensaje encriptado usando el año de nacimiento
        /// </summary>
        /// <param name="encryptedMessage">Valores encriptados separados por espacios</param>
        /// <param name="birthYear">El año de nacimiento para la desencriptación</param>
        /// <returns>El mensaje desencriptado</returns>
        static string Decrypt(string encryptedMessage, int birthYear)
        {
            StringBuilder decrypted = new StringBuilder();

            string[] encryptedValues = encryptedMessage.Split(' ');

            for (int i = 0; i < encryptedValues.Length; i++)
            {
                if (int.TryParse(encryptedValues[i], out int value))
                {
                    if (value == -1 && i + 1 < encryptedValues.Length)
                    {
                        // Este es un indicador especial para caracteres Unicode altos
                        // El siguiente valor es el valor Unicode directo
                        i++; // Avanzar al siguiente valor (el valor Unicode)

                        if (int.TryParse(encryptedValues[i], out int unicodeValue))
                        {
                            // Convertir directamente el valor Unicode a carácter
                            char c = (char)unicodeValue;
                            decrypted.Append(c);
                        }
                        else
                        {
                            // Si hay un error en el formato
                            decrypted.Append("?");
                        }
                    }
                    else
                    {
                        // Método normal de desencriptación
                        int unicodeValue = birthYear - value;
                        if (unicodeValue >= 0) // Los caracteres Unicode válidos tienen valores no negativos
                        {
                            char c = (char)unicodeValue;
                            decrypted.Append(c);
                        }
                        else
                        {
                            // Si el resultado es negativo, no es un carácter válido
                            decrypted.Append("?");
                        }
                    }
                }
                else
                {
                    // Si la conversión falla, agregar un marcador
                    decrypted.Append("?");
                }
            }

            return decrypted.ToString();
        }
    }
}