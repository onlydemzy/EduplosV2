using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Eduplus.Services.UtilityServices
{
    internal static class StandardGeneralOps
    {
        internal static string ToTitleCase(string value)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (string.IsNullOrEmpty(value))
                return "";

            string sn = value.ToLower();
            return textInfo.ToTitleCase(sn);

        }

        internal static string GeneratePersonId(int? session)
        {
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] salt = new byte[16];
            random.GetBytes(salt);
            StringBuilder sbuilder = new StringBuilder(salt.Length * 2);
            string neededSession = session.ToString();
            
            foreach (byte b in salt)
            {
                sbuilder.Append(b.ToString("x2"));
            }
            string finalNumber = sbuilder.ToString();
            int stLength = finalNumber.Length - 9;
            string wantedstring = finalNumber.Substring(stLength, 9);

            if(session>0)
                return session.ToString()+wantedstring.ToUpper();
            else
                return "US" + wantedstring.ToUpper();
           
        }
        internal static string ToMoney(double amount)
        {
            // amount.ToString("#,##0.00;\(#,##0.00\)")
            return string.Format("{0:#,##0.00;(#,##0.00)}", amount);
        }

        
        internal static int GenereteSessionId(string session)
        {
            string id = session.Substring(2, 2);
            return Convert.ToInt32(id);
        }

        
        internal static int GenerateSemesterId(string semester,int sessionId)
        {
            int id;
            switch (semester)
            {
                case "1st Semester":
                    id=Convert.ToInt32(sessionId.ToString() + 1);
                    break;
                case "2nd Semester":
                    id= Convert.ToInt32(sessionId.ToString() + 2);
                    break;
                case "3rd Semester":
                    id = Convert.ToInt32(sessionId.ToString() + 3);
                    break;
                default:
                    id = 0;
                    break;
            }
            return id;
        }

      public static DateTime ConvertUtcToLocalTimeZone(DateTime utc)
        {
            var tm = TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));
            
            return tm;
        }
    }
}
