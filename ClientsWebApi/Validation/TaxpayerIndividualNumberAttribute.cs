using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ClientsWebApi.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class TaxpayerIndividualNumberAttribute: ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value is string TIN)
            {
                var regex = new Regex(@"^([0-9]{10})|([0-9]{12})$");

                return regex.IsMatch(TIN);
            }
            return false;
        }
    }
}
