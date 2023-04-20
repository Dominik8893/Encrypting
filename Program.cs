using System;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    static void Main()
    {
        string FilePath = "crypto.txt";


        while (true)
        {
            Console.Clear();
            Console.WriteLine("Press 1 to add a new encrypted line\nPress 2 to read your Decrypted File\nPress 3 to encrypt a file provided a filepath and a key\nPress 4 to decrypt a file provided a filepath and a key\nPress 5 to Exit");

            int UserSelection = int.Parse(Console.ReadLine());

            switch (UserSelection)
            {
                case 1:
                    {
                        AddEncryptedLine(FilePath);
                        break;
                    }
                case 2:
                    {
                        ReadEncryptedFile(FilePath);
                        break;
                    }
                case 3:
                    {
                        EncryptFile();
                        break;
                    }
                case 4:
                    {
                        DecryptFile();
                        break;
                    }
                case 5:
                    {

                        return;
                    }
            }

        }



        //Console.WriteLine("Enter Your Encryption Key");
        //string Key = Console.ReadLine();
        //// The key to use for encryption (must be 16, 24, or 32 bytes long)
        //byte[] ByteKey = Encoding.UTF8.GetBytes(Key);
        //int KeyLength = ByteKey.Length;
        //Console.WriteLine("{0},  Length:{1}", ByteKey, KeyLength);

        //string[] FileLines = File.ReadAllLines(FilePath);

        //foreach (string Line in FileLines)
        //{
        //    DecryptString(Line, ByteKey);
        //    Console.WriteLine(Line);
        //}
    }

    static string EncryptString(string inputString, byte[] ByteKey)
    {
        // Create a new instance of the AES algorithm
        using (Aes aes = Aes.Create())
        {
            // Set the key and initialization vector (IV) for the AES algorithm
            aes.Key = ByteKey;
            aes.GenerateIV();

            // Convert the input string to a byte array
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);

            // Create a new instance of the AES encryptor
            ICryptoTransform encryptor = aes.CreateEncryptor();

            // Encrypt the input byte array
            byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            // Combine the IV and encrypted bytes into a single byte array
            byte[] ivAndEncryptedBytes = new byte[aes.IV.Length + encryptedBytes.Length];
            Array.Copy(aes.IV, 0, ivAndEncryptedBytes, 0, aes.IV.Length);
            Array.Copy(encryptedBytes, 0, ivAndEncryptedBytes, aes.IV.Length, encryptedBytes.Length);

            // Convert the combined byte array to a base64-encoded string
            string encryptedString = Convert.ToBase64String(ivAndEncryptedBytes);

            return encryptedString;
        }
    }

    static string DecryptString(string encryptedString, byte[] ByteKey)
    {
        // Convert the base64-encoded string to a byte array

        if(string.IsNullOrEmpty(encryptedString))
        {
            return " ";
        }

        byte[] ivAndEncryptedBytes = Convert.FromBase64String(encryptedString);

        // Create a new instance of the AES algorithm
        using (Aes aes = Aes.Create())
        {
            // Set the key and initialization vector (IV) for the AES algorithm
            aes.Key = ByteKey;
            byte[] iv = new byte[aes.IV.Length];
            Array.Copy(ivAndEncryptedBytes, 0, iv, 0, iv.Length);
            aes.IV = iv;

            // Create a new instance of the AES decryptor
            ICryptoTransform decryptor = aes.CreateDecryptor();

            // Decrypt the encrypted byte array
            byte[] decryptedBytes = decryptor.TransformFinalBlock(ivAndEncryptedBytes, aes.IV.Length, ivAndEncryptedBytes.Length - aes.IV.Length);

            // Convert the decrypted byte array to a string
            string decryptedString = Encoding.UTF8.GetString(decryptedBytes);

            return decryptedString;
        }
    }
    static void AddEncryptedLine(string FilePath)
    {
        Console.WriteLine("Enter the String to Encrypt: ");
        string StringToEncrypt = Console.ReadLine();
        Console.WriteLine("Enter Your Encryption Key");
        string Key = Console.ReadLine();
        byte[] ByteKey = Encoding.UTF8.GetBytes(Key);
        string EncryptedString = EncryptString(StringToEncrypt, ByteKey);

        File.AppendAllText(FilePath, Environment.NewLine + EncryptedString);

        return;
    }
    static void ReadEncryptedFile(string FilePath)
    {
        Console.WriteLine("Enter Your Encryption Key");
        string Key = Console.ReadLine();
        byte[] ByteKey = Encoding.UTF8.GetBytes(Key);

        string[] FileLines = File.ReadAllLines(FilePath);

        foreach (string Line in FileLines)
        {
            string DecryptedSring = DecryptString(Line, ByteKey);
            Console.WriteLine(DecryptedSring);
        }

    }
    static void DecryptFile()
    {
        Console.WriteLine("Enter the Encrypted Files FilePath");
        string EncryptedFilePath = Console.ReadLine();

        Console.WriteLine("Enter the Decrypted Files FilePath");
        string DecryptedFilePath = Console.ReadLine();

        Console.WriteLine("Enter Your Encryption Key");
        string Key = Console.ReadLine();
        byte[] ByteKey = Encoding.UTF8.GetBytes(Key);

        string[] FileLines = File.ReadAllLines(EncryptedFilePath);

        foreach (string Line in FileLines)
        {
            File.AppendAllText(DecryptedFilePath, Environment.NewLine + DecryptString(Line, ByteKey)); 
        }

    }
    static void EncryptFile()
    {
        
        Console.WriteLine("Enter the Decrypted Files FilePath");
        string DecryptedFilePath = Console.ReadLine();

        Console.WriteLine("Enter the Encrypted Files FilePath");
        string EncryptedFilePath = Console.ReadLine();

        Console.WriteLine("Enter Your Encryption Key");
        string Key = Console.ReadLine();
        byte[] ByteKey = Encoding.UTF8.GetBytes(Key);

        string[] FileLines = File.ReadAllLines(DecryptedFilePath);

        foreach (string Line in FileLines)
        {
            File.AppendAllText(EncryptedFilePath, Environment.NewLine + EncryptString(Line, ByteKey));
        }

    }

}