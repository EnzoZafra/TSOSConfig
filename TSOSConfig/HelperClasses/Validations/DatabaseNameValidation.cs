using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using TSOSConfig.ViewModels;

namespace TSOSConfig.HelperClasses.Validations
{
    class DatabaseNameValidation : ValidationRule
    {
        private const int MAX_DATABASENAME = 3;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            Regex regex = new Regex(@"^[a-zA-Z0-9]*$");
            List<string> databasenames =
                ConfigureViewModel.Instance.DatabaseList.Select(db => db.DatabaseName.ToLower()).ToList();

            if (string.IsNullOrWhiteSpace((string)value))
            {
                return new ValidationResult(false, "This field cannot be blank.");
            }
            else if (((string)value).Length > MAX_DATABASENAME)
            {
                return new ValidationResult(false, "Database names cannot be longer than 3 characters.");
            }
            else if (!(regex.IsMatch((string)value)))
            {
                return new ValidationResult(false, "Database names can only contain alphanumeric characters.");
            }
            else if (databasenames.Contains(((string) value).ToLower()))
            {
                return new ValidationResult(false, "Database name already exists.");
            }

            return new ValidationResult(true, null);
        }
    }
}
