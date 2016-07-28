using System.Globalization;
using System.Windows.Controls;

namespace TSOSConfig.HelperClasses.Validations
{
    class RecurringTimeValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "This field cannot be blank.");
            }
            else if ((int)value == 0)
            {
                return new ValidationResult(false, "This field cannot be 0.");
            }

            else if (value.GetType() != typeof(int))
            {
                return new ValidationResult(false, "Please enter an integer.");
            }

            return new ValidationResult(true, null);
        }
    }
}
