using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HorseLeague.Services
{
    public interface ICaptchaService
    {
        bool IsValid(string captcha);
    }
}
