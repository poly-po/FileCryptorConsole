using System;
using System.IO;
using System.Security.Cryptography;

namespace FileCryptorConsole.Services
{
    public class CryptoService
    {
        private readonly RSA _rsa;
        private const int BlockSize = 128; // Размер блока для RSA (меньше для простоты)

        public CryptoService()
        {
            _rsa = RSA.Create(2048); // Просто создаём RSA с ключом 2048 бит
        }

        // Шифрование файла по блокам
        public void EncryptFile(string inputPath, string outputPath)
        {
            using var inputFile = File.OpenRead(inputPath);
            using var outputFile = File.Create(outputPath);

            byte[] buffer = new byte[BlockSize];
            int bytesRead;

            while ((bytesRead = inputFile.Read(buffer, 0, BlockSize)) > 0)
            {
                byte[] encryptedBlock = _rsa.Encrypt(buffer.AsSpan(0, bytesRead).ToArray(), RSAEncryptionPadding.Pkcs1);
                outputFile.Write(encryptedBlock, 0, encryptedBlock.Length);
            }
        }

        // Дешифрование файла по блокам
        public void DecryptFile(string inputPath, string outputPath)
        {
            using var inputFile = File.OpenRead(inputPath);
            using var outputFile = File.Create(outputPath);

            byte[] buffer = new byte[256]; // Размер зашифрованного блока RSA 2048
            int bytesRead;

            while ((bytesRead = inputFile.Read(buffer, 0, 256)) > 0)
            {
                byte[] decryptedBlock = _rsa.Decrypt(buffer.AsSpan(0, bytesRead).ToArray(), RSAEncryptionPadding.Pkcs1);
                outputFile.Write(decryptedBlock, 0, decryptedBlock.Length);
            }
        }

        // Подпись файла MD5
        public byte[] SignFile(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hash = md5.ComputeHash(stream);
            return _rsa.SignHash(hash, HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
        }

        // Проверка подписи
        public bool VerifySignature(string filePath, byte[] signature)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hash = md5.ComputeHash(stream);
            return _rsa.VerifyHash(hash, signature, HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
        }
    }
}