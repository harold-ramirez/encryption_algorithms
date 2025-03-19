using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Cryptography;


namespace encryption_algorithms
{
    static void Main()
{
    using (RSA rsa = RSA.Create(2048))
    {
        
        byte[] data = Encoding.UTF8.GetBytes("Hola mundo de pruebita");

        // Medicion de uso de memoria
        long initialMemory = GC.GetTotalMemory(true);

        // Tiempos de ejecucion
        double[] encryptionTimes = new double[1000];
        double[] decryptionTimes = new double[1000];

        // Cifrado 1000 veces
        byte[] encryptedData = null;
        for (int i = 0; i < 1000; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
            sw.Stop();
            encryptionTimes[i] = sw.Elapsed.TotalMilliseconds;
        }

        // Descifrado 1000 veces
        byte[] decryptedData = null;
        for (int i = 0; i < 1000; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.OaepSHA256);
            sw.Stop();
            decryptionTimes[i] = sw.Elapsed.TotalMilliseconds;
        }

        // Memoria final
        long finalMemory = GC.GetTotalMemory(true);
        double memoryUsedKB = (finalMemory - initialMemory) / 1024.0;

        // Estadisticas
        double totalEncTime = Sum(encryptionTimes);
        double avgEncTime = totalEncTime / 1000;
        double totalDecTime = Sum(decryptionTimes);
        double avgDecTime = totalDecTime / 1000;

        // Resultados
        Console.WriteLine("Cifrado completado.");
        Console.WriteLine("\nEstadísticas de ejecución:");
        Console.WriteLine($"├── Tiempo total de encriptación: {totalEncTime:F3} ms");
        Console.WriteLine($"│   └── Tiempo promedio por iteración: {avgEncTime:F6} ms");
        Console.WriteLine($"├── Tiempo total de desencriptación: {totalDecTime:F3} ms");
        Console.WriteLine($"│   └── Tiempo promedio por iteración: {avgDecTime:F6} ms");
        Console.WriteLine($"├── Memoria utilizada: {memoryUsedKB:F3} KB");
        Console.WriteLine($"└── Carga promedio del procesador: {Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds / (totalEncTime + totalDecTime):F6} ms");
    }
}

static double Sum(double[] values)
{
    double sum = 0;
    foreach (double val in values) sum += val;
    return sum;
}
}
