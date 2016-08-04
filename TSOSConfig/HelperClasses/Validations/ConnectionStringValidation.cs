using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace TSOSConfig.HelperClasses.Validations
{
    class ConnectionStringValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9.\-_;,$]*$");
            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, "This field cannot be blank.");
            }
            else if (!(regex.IsMatch((string)value)))
            {
                return new ValidationResult(false, "Customer Names can only contain alphanumeric characters and (._-$;,)");
            }

            return new ValidationResult(true, null);
        }
    }
}
