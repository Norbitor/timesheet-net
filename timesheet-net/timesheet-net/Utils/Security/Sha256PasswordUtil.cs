using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace timesheet_net.Utils.Security
{
    public class Sha256PasswordUtil
    {
        public string hash(string password)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] hashPass = sha256.ComputeHash(Encoding.Default.GetBytes(password)); //256-bits employee pass
            return BitConverter.ToString(hashPass).Replace("-", string.Empty); //64 chars hash pass
        }

        public bool verify(string password, string hash)
        {
            string passHash = this.hash(password);
            return passHash.Equals(hash);
        }
    }
}