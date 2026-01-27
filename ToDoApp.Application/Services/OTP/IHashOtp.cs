using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.Services.OTP
{
    public interface IHashOtp
    {
        string Hashotp(string otp);
        string GenerateOtp();
    }
}
