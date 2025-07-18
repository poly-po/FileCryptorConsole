using System;
using System.Security.Cryptography;
using FileCryptorConsole.Services;

class Program
{
    static void Main()
    {
        var cryptoService = new CryptoService();
        var fileService = new FileService();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== File Cryptor ===");
            Console.WriteLine("1. Зашифровать файл (RSA)");
            Console.WriteLine("2. Расшифровать файл (RSA)");
            Console.WriteLine("3. Подписать файл (MD5 + RSA)");
            Console.WriteLine("4. Проверить подпись");
            Console.WriteLine("5. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    EncryptFile(cryptoService, fileService);
                    break;
                case "2":
                    DecryptFile(cryptoService, fileService);
                    break;
                case "3":
                    SignFile(cryptoService, fileService);
                    break;
                case "4":
                    VerifySignature(cryptoService, fileService);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
            Console.WriteLine("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }

    static void EncryptFile(CryptoService crypto, FileService fileService)
    {
        Console.Write("Введите путь к файлу: ");
        string inputPath = Console.ReadLine().Trim();

        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Файл не найден!");
            return;
        }

        string outputPath = inputPath + ".encrypted";

        try
        {
            crypto.EncryptFile(inputPath, outputPath); // Просто вызываем метод
            Console.WriteLine($"Файл зашифрован: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка шифрования: {ex.Message}");
            if (File.Exists(outputPath)) File.Delete(outputPath);
        }
    }

    static void DecryptFile(CryptoService crypto, FileService fileService)
    {
        Console.Write("Введите путь к зашифрованному файлу: ");
        string inputPath = Console.ReadLine().Trim();

        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Файл не найден!");
            return;
        }

        string outputPath = inputPath.Replace(".encrypted", "");

        try
        {
            crypto.DecryptFile(inputPath, outputPath);
            Console.WriteLine($"Файл расшифрован: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка дешифрования: {ex.Message}");
            if (File.Exists(outputPath)) File.Delete(outputPath);
        }
    }

    static void SignFile(CryptoService crypto, FileService fileService)
    {
        Console.Write("Введите путь к файлу: ");
        string filePath = Console.ReadLine().Trim();

        try
        {
            byte[] signature = crypto.SignFile(filePath);
            fileService.SaveFile(filePath + ".signature", signature);
            Console.WriteLine("Подпись создана успешно");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка создания подписи: {ex.Message}");
        }
    }

    static void VerifySignature(CryptoService crypto, FileService fileService)
    {
        Console.Write("Введите путь к файлу: ");
        string filePath = Console.ReadLine().Trim();

        Console.Write("Введите путь к подписи: ");
        string signaturePath = Console.ReadLine().Trim();

        try
        {
            byte[] signature = fileService.ReadFile(signaturePath);
            bool isValid = crypto.VerifySignature(filePath, signature); // Исправленное имя метода
            Console.WriteLine($"Подпись {(isValid ? "валидна" : "недействительна")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка проверки подписи: {ex.Message}");
        }
    }
}