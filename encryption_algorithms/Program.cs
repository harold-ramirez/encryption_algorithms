using System;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;

namespace encryption_algorithms
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            // Ruta del archivo de texto
            string filePath = "Practica 2_02_PauloCoelho-ElAlquimista.txt";
            string key = "claveSecreta"; // Clave utilizada para la encriptación XOR

            // Leer todo el contenido del archivo
            string fileContent = File.ReadAllText(filePath, Encoding.UTF8);
            Console.WriteLine("Contenido leído del archivo:");
            //Console.WriteLine("Mensaje original:\n"+fileContent);  // Muestra solo los primeros 200 caracteres para verificar

            // Ejecutar el proceso de encriptación y desencriptación 1000 veces
            int iterations = 1000;
            Stopwatch stopwatch = Stopwatch.StartNew();
            long initialMemory = Process.GetCurrentProcess().PrivateMemorySize64;
            double initialCpu = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
            string encrypted = XOREncryptDecrypt(fileContent, key);
            //Console.WriteLine("\nMensaje encriptado:\n"+encrypted.Substring(0, Math.Min(encrypted.Length, 200)) + "...");
            string decrypted = XOREncryptDecrypt(encrypted, key);
            //Console.WriteLine(decrypted.Substring(0, Math.Min(decrypted.Length, 200)) + "...");  // Muestra solo los primeros 200 caracteres para verificar

            // Ejecutar el algoritmo Diffie-Hellman en 1000 iteraciones
            for (int i = 0; i < iterations; i++)
            {
                encrypted = XOREncryptDecrypt(decrypted, key);
                decrypted = XOREncryptDecrypt(encrypted, key);
                Console.WriteLine("iteracion:" + i);
            }

            stopwatch.Stop();
            long finalMemory = Process.GetCurrentProcess().PrivateMemorySize64;
            double finalCpu = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;

            // Medir el rendimiento
            Console.WriteLine("\nMensaje desencriptado:\n"+decrypted);
            Console.WriteLine($"\nTiempo total de ejecución: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Tiempo promedio por iteración: {stopwatch.ElapsedMilliseconds / iterations} ms");
            Console.WriteLine($"Memoria utilizada: {(finalMemory - initialMemory) / 1024} KB");
            Console.WriteLine($"Uso de CPU: {finalCpu - initialCpu} ms");
        }

        static string XOREncryptDecrypt(string input, string key)
        {
            StringBuilder output = new StringBuilder();

            // Loop through each character in the input
            for (int i = 0; i < input.Length; i++)
            {
                // XOR the input character with the corresponding key character
                char inputChar = input[i];
                char keyChar = key[i % key.Length]; // Reuse the key if it's shorter than the input
                char encryptedChar = (char)(inputChar ^ keyChar); // XOR operation
                output.Append(encryptedChar);
            }

            return output.ToString();
        }
    }
}
