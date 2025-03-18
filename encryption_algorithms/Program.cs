using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace encryption_algorithms
{
    class Program
    {
        // Número de iteraciones para las pruebas
        private const int ITERATIONS = 1000;
        // Número de mediciones a realizar
        private const int CPU_SAMPLE_COUNT = 10;

        static void Main(string[] args)
        {
            Console.WriteLine("Evaluación de seguridad y comparación de algoritmos de encriptación");
            Console.WriteLine("==================================================================");

            try
            {
                // Ruta del archivo (ajusta esto a tu ruta específica)
                string filePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "texto prueba AES.txt");

                // Leer el contenido del archivo
                string plainText = File.ReadAllText(filePath);
                Console.WriteLine($"Archivo cargado: {filePath}");
                Console.WriteLine($"Tamaño del texto: {plainText.Length} caracteres");

                // Generar clave y vector de inicialización
                byte[] key = GenerateRandomBytes(32); // 256 bits
                byte[] iv = GenerateRandomBytes(16);  // 128 bits

                Console.WriteLine("\nIniciando evaluación de AES (1000 iteraciones)...");
                Console.WriteLine("Por favor espere...");

                // Lista para almacenar las mediciones de CPU
                List<double> cpuUsageList = new List<double>();

                // Generar datos aleatorios para calentar la caché
                WarmupCache();

                // Iniciar medición de rendimiento
                var stopwatch = Stopwatch.StartNew();
                var process = Process.GetCurrentProcess();
                var startMemory = process.WorkingSet64;

                // Registrar tiempo de CPU inicial
                process.Refresh();
                var startTime = DateTime.Now;
                var startCpuTime = process.TotalProcessorTime;

                byte[] encryptedData = null;
                string decryptedText = null;

                // Calcular el número de iteraciones por muestra
                int iterationsPerSample = ITERATIONS / CPU_SAMPLE_COUNT;

                // Ejecutar ciclo de iteraciones
                for (int i = 0; i < ITERATIONS; i++)
                {
                    // Encriptar
                    encryptedData = EncryptAES(plainText, key, iv);

                    // Desencriptar
                    decryptedText = DecryptAES(encryptedData, key, iv);

                    // Verificar integridad cada 100 iteraciones para no afectar rendimiento
                    if (i % 100 == 0)
                    {
                        if (plainText != decryptedText)
                        {
                            throw new Exception("¡Error! El texto desencriptado no coincide con el original");
                        }
                    }

                    // Medir CPU en puntos específicos del ciclo
                    if (i % iterationsPerSample == 0)
                    {
                        // Tomar una muestra de CPU
                        process.Refresh();
                        var currentTime = DateTime.Now;
                        var currentCpuTime = process.TotalProcessorTime;

                        double cpuUsedMs = (currentCpuTime - startCpuTime).TotalMilliseconds;
                        double totalElapsedMs = (currentTime - startTime).TotalMilliseconds;

                        // Calcular el porcentaje de CPU usado
                        double cpuUsagePercent = (cpuUsedMs / (Environment.ProcessorCount * totalElapsedMs)) * 100.0;
                        cpuUsageList.Add(cpuUsagePercent);

                        // Actualizar tiempos para la próxima medición
                        startTime = currentTime;
                        startCpuTime = currentCpuTime;
                    }
                }

                stopwatch.Stop();

                // Medir memoria al final
                process.Refresh();
                var endMemory = process.WorkingSet64;
                var endCpuTime = process.TotalProcessorTime;

                // Calcular y mostrar resultados
                Console.WriteLine("\nResultados de la evaluación:");
                Console.WriteLine("---------------------------");
                Console.WriteLine($"Tiempo total: {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Tiempo promedio por iteración: {stopwatch.ElapsedMilliseconds / (double)ITERATIONS:F3} ms");
                Console.WriteLine($"Memoria utilizada: {(endMemory - startMemory) / 1024.0:F2} KB");

                // Mostrar estadísticas de uso de CPU
                if (cpuUsageList.Count > 0)
                {
                    double avgCpu = cpuUsageList.Average();
                    double maxCpu = cpuUsageList.Max();
                    Console.WriteLine($"Carga promedio del procesador: {avgCpu:F2}%");
                    Console.WriteLine($"Carga máxima del procesador: {maxCpu:F2}%");
                    Console.WriteLine($"Muestras de CPU tomadas: {cpuUsageList.Count}");
                }

                Console.WriteLine($"Tamaño del texto original: {plainText.Length} bytes");
                Console.WriteLine($"Tamaño del texto encriptado: {encryptedData.Length} bytes");

                // Verificación final
                Console.WriteLine("\nVerificación de integridad: " +
                                 (plainText == decryptedText ? "EXITOSA ✓" : "FALLIDA ✗"));

                Console.WriteLine("\nPrueba completada. Presiona cualquier tecla para salir...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Calienta la caché del sistema para obtener mediciones más precisas
        /// </summary>
        private static void WarmupCache()
        {
            // Realizar algunas operaciones para calentar la caché
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                byte[] dummyData = new byte[1024];
                new Random().NextBytes(dummyData);

                ICryptoTransform encryptor = aes.CreateEncryptor();
                encryptor.TransformBlock(dummyData, 0, dummyData.Length, dummyData, 0);
            }
        }

        /// <summary>
        /// Genera un array de bytes aleatorio del tamaño especificado
        /// </summary>
        private static byte[] GenerateRandomBytes(int size)
        {
            byte[] randomBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        /// <summary>
        /// Encripta un texto utilizando AES
        /// </summary>
        private static byte[] EncryptAES(string plainText, byte[] key, byte[] iv)
        {
            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Crear encriptador
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        encrypted = memoryStream.ToArray();
                    }
                }
            }

            return encrypted;
        }

        /// <summary>
        /// Desencripta datos encriptados con AES
        /// </summary>
        private static string DecryptAES(byte[] cipherText, byte[] key, byte[] iv)
        {
            string plaintext = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Crear desencriptador
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            plaintext = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}