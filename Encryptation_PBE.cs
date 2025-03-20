using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class PBEEncryptionTest
{
    private const int KeySize = 256;
    private const int Iterations = 10000;
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("S@ltV@lu3");

    // Función para derivar clave desde una contraseña
    private static byte[] DeriveKey(string password)
    {
        using (var deriveBytes = new Rfc2898DeriveBytes(password, Salt, Iterations, HashAlgorithmName.SHA256))
        {
            return deriveBytes.GetBytes(KeySize / 8);
        }
    }

    // Cifrar texto con PBE y AES
    public static byte[] Encrypt(string plaintext, string password)
    {
        byte[] key = DeriveKey(password);
        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();
            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs))
                {
                    writer.Write(plaintext);
                }
                return ms.ToArray();
            }
        }
    }

    // Descifrar texto con PBE y AES
    public static string Decrypt(byte[] ciphertext, string password)
    {
        byte[] key = DeriveKey(password);
        using (var aes = Aes.Create())
        {
            using (var ms = new MemoryStream(ciphertext))
            {
                byte[] iv = new byte[aes.IV.Length];
                ms.Read(iv, 0, iv.Length);
                aes.Key = key;
                aes.IV = iv;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    static void Main()
    {
        string password = "MiClaveSegura";
        string filePath = "Practica 2_02_PauloCoelho-ElAlquimista.txt";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Archivo no encontrado.");
            return;
        }

        string text = File.ReadAllText(filePath);
        Stopwatch stopwatch = new Stopwatch();
        long initialMemory = GC.GetTotalMemory(true);
        double totalCpuUsage = 0;

        for (int i = 0; i < 1000; i++)
        {
            stopwatch.Start();
            byte[] encryptedData = Encrypt(text, password);
            string decryptedText = Decrypt(encryptedData, password);
            stopwatch.Stop();
            totalCpuUsage += Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
        }

        long finalMemory = GC.GetTotalMemory(true);
        Console.WriteLine($"Tiempo total (1000): {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"Tiempo promedio por iteración: {stopwatch.ElapsedMilliseconds / 1000.0} ms");
        Console.WriteLine($"Memoria usada: {(finalMemory - initialMemory) / 1024} KB");
        Console.WriteLine($"Carga de CPU total: {totalCpuUsage} ms");
    }
}
