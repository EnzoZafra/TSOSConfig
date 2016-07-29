using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TSOSConfig.ViewModels;

namespace TSOSConfig.HelperClasses.Validations
{
    class CustomerNameValidation : ValidationRule
    {
        private const int MAX_CUSTOMERNAME = 50;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9.\-_$]*$");
            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, "This field cannot be blank.");
            }
            else if (((string)value).Length > MAX_CUSTOMERNAME)
            {
                return new ValidationResult(false, "The name cannot be any longer than 50 characters.");
            }
            else if (!(regex.IsMatch((string)value)))
            {
                return new ValidationResult(false, "Customer Names can only contain alphanumeric characters and (._-$)");
            }

            return new ValidationResult(true, null);
        }
    }
}
