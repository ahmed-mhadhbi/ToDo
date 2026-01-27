using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.Services.OTP
{
    public class HashOtp : IHashOtp
    {
        public HashOtp(){ 
        }

        // Instance implementation for IHashOtp
        public string Hashotp(string otp)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(otp);
            return System.Convert.ToBase64String(sha.ComputeHash(bytes));
        }

        public string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();
        }

       
    }
}


