using System;
namespace HorseLeague.Services
{
    public interface IPaypalService
    {
        string GenerateCallbackValue(IEncryptor encryptor, HorseLeague.Models.Domain.UserLeague userLeague);
        PaypalDTO UnpackCallback(IEncryptor encryptor, string callback);
    }
}
