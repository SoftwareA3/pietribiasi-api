using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Validation
{
    public static class GenericValidation
    {
        public static void ValidateOnlyAllowedProperties<T>(this T request, params string[] allowedPropertyNames)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }
            var properties = request.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (!allowedPropertyNames.Contains(property.Name) && property.GetValue(request) != null)
                {
                    throw new ArgumentException($"Property {property.Name} must be null when not explicitly allowed for this request type.");
                }
            }
        }
    }
}