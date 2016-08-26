using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace TSOSConfig.HelperClasses.Validations
{
    class ConnectionStringValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9.=\-;]*$");
            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, "This field cannot be blank.");
            }
            else if (!(regex.IsMatch((string)value)))
            {
                return new ValidationResult(false, "MySQL Connection Strings can only contain alphanumeric characters and (=.-;)");
            }

            return new ValidationResult(true, null);
        }
    }
}
