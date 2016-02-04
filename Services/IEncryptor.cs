using System;
namespace HorseLeague.Services
{
    public interface IEncryptor
    {
        string Decrypt(string cipherText);
        string Encrypt(string clearText);
    }
}
