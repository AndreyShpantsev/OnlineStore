using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace WebApplicationTechSale.HelperServices
{
    public class ExtensionValidationAttribute : ValidationAttribute
    {
        private readonly string[] acceptableExtensions;

        public ExtensionValidationAttribute(string[] acceptableExtensions)
        {
            this.acceptableExtensions = acceptableExtensions;
        }

        public override bool IsValid(object value)
        {
            if (value is IFormFile)
            {
                string extension = Path.GetExtension((value as IFormFile).FileName);
                foreach (var acceptableExtension in acceptableExtensions) 
                {
                    if (extension == acceptableExtension)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
