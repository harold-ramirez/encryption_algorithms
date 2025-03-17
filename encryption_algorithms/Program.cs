using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using System.CodeDom;
using System.Threading;

namespace encryption_algorithms
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int iterations = 1000;
            string filePath = "C:/Users/LENOVO/Downloads/ElAlquimista.txt"; // Ruta del archivo de texto
            if (!File.Exists(filePath))
            {
                Console.WriteLine("El archivo no existe.");
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            long memoriaAntes = GC.GetTotalMemory(true);
            double totalCpuUsage = 0;

            Process proceso = Process.GetCurrentProcess();

            stopwatch.Start();
            string hash = File.ReadAllText(filePath);

            for (int i = 0; i < iterations; i++)
            {
                Console.Write($"Encriptando texto: {i+1} de {iterations}...\r");
                hash = SHA3_256(hash);
                totalCpuUsage += proceso.TotalProcessorTime.TotalMilliseconds;
            }
            Console.WriteLine("\nCifrado completado.");

            stopwatch.Stop();
            long memoriaDespues = GC.GetTotalMemory(true);
            double tiempoTotal = stopwatch.ElapsedMilliseconds;
            double tiempoPromedio = tiempoTotal / iterations;

            Console.WriteLine($"\nHash final ({iterations} iteraciones):\n" + hash);
            Console.WriteLine("\nEstadísticas de ejecución:");
            Console.WriteLine($"├─ Tiempo total:");
            Console.WriteLine($"│  ├─ {tiempoTotal} ms");
            Console.WriteLine($"│  └─ {Math.Round(tiempoTotal / 1000.0, 4)} s");
            Console.WriteLine($"├─ Tiempo promedio por iteración:");
            Console.WriteLine($"│  ├─ {tiempoPromedio} ms");
            Console.WriteLine($"│  └─ {Math.Round(tiempoPromedio / 1000.0, 4)} s");
            Console.WriteLine($"├─ Memoria utilizada:");
            Console.WriteLine($"│  ├─ {Math.Round((memoriaDespues - memoriaAntes) / 1024.0, 4)} KB ");
            Console.WriteLine($"│  └─ {Math.Round((memoriaDespues - memoriaAntes) / (1024.0 * 1024.0), 4)} MB");
            Console.WriteLine($"└─ Carga promedio del procesador:");
            Console.WriteLine($"   ├─ {Math.Round(totalCpuUsage / iterations, 4)} ms");
            Console.WriteLine($"   └─ {Math.Round((totalCpuUsage / iterations) / 1000.0, 4)} s");
            Console.ReadLine();
        }

        static string SHA3_256(string input)
        {
            Sha3Digest sha3 = new Sha3Digest(256);
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = new byte[sha3.GetDigestSize()];

            sha3.BlockUpdate(inputBytes, 0, inputBytes.Length);
            sha3.DoFinal(hashBytes, 0);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
