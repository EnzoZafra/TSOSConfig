using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace TSOSConfig.HelperClasses.Validations
{
    class MailServerValidation : ValidationRule
    {
        // Checks if the input has 3 decimal points and only have digits.
        private bool PossibleIPAddress(string input)
        {
            Regex regex = new Regex(@"^[0-9.]*$");
            int decimalpoints = input.Count(x => x == '.');

            return regex.IsMatch(input) && decimalpoints == 3;
        }

        public static bool CheckIPValid(string strIP)
        {
            string[] byteblocks = strIP.Split('.');
            for (int i = 0; i < 4; i++)
            {
                if(string.IsNullOrWhiteSpace(byteblocks[i]))
                {
                    continue;
                }

                int block = int.Parse(byteblocks[i]);
                // Each block can only have 8 bits ie, between 0 or 255.
                if (block < 0 || block > 255)
                {
                    return false;
                }
            }
            return true;
        }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9.]*$");

            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, "This field cannot be blank.");
            }
            else if (!(regex.IsMatch((string)value)))
            {
                return new ValidationResult(false, "Mail Servers cannot contain special characters.");
            }
            else if (PossibleIPAddress((string)value))
            {
                if (!CheckIPValid((string) value))
                {
                    return new ValidationResult(false, "Invald IP Address.");
                }
            }

            return new ValidationResult(true, null);
        }
    }
}
