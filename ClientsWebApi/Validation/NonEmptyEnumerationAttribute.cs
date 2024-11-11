using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NonEmptyEnumerationAttribute: ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value is IEnumerable enumetion)
            {
                return enumetion.GetEnumerator().MoveNext();
            }
            return false;
        }
    }
}
