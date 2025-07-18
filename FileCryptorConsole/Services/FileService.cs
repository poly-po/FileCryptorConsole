using System;
using System.IO;

namespace FileCryptorConsole.Services
{
    public class FileService
    {
        public byte[] ReadFile(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception ex) when (ex is FileNotFoundException ||
                                     ex is DirectoryNotFoundException ||
                                     ex is UnauthorizedAccessException)
            {
                Console.WriteLine($"Ошибка чтения файла: {ex.Message}");
                throw; // Перебрасываем исключение для обработки в Program.cs
            }
        }

        public void SaveFile(string defaultPath, byte[] data)
        {
            string outputPath = GetOutputPath(defaultPath);

            try
            {
                EnsureDirectoryExists(outputPath);
                File.WriteAllBytes(outputPath, data);
                Console.WriteLine($"Файл успешно сохранён: {outputPath}");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Ошибка: Нет прав для записи в указанную директорию!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        private string GetOutputPath(string defaultPath)
        {
            Console.Write($"Введите путь для сохранения (по умолчанию: {defaultPath}): ");
            string customPath = Console.ReadLine()?.Trim();
            return string.IsNullOrWhiteSpace(customPath) ? defaultPath : customPath;
        }

        private void EnsureDirectoryExists(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}