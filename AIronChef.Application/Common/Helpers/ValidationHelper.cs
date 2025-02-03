using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AIronChef.Application.Common.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if(string.IsNullOrWhiteSpace(email))
                return false;

            int atPos = email.IndexOf('@');
            if (atPos <= 0)
                return false;
            // local part should be max 64 characters
            if (atPos > 64)
                return false;
            // local part cannot start with .
            if (email[0] == '.')
                return false;
            // local part cannot end with .
            if (email[atPos - 1] == '.')
                return false;
            // local part cannot have two consecutive .'s, probably not domain either
            if (email.IndexOf("..") >= 0)
                return false;
            if (email.IndexOf('"') >= 0)
            {
                if (email[0] == '"' && email[atPos - 1] == '"')
                {
                    // quoted local part should be fine
                }
                else
                {
                    return false;
                }
            }
            // Top level domain should be minimum 2 characters
            if (email.LastIndexOf('.') > email.Length - 3)
            {
                return false;
            }

            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNonEmpty(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        public static bool IsIdValid(int id)
        {
            return id > 0;
        }
    }
}
