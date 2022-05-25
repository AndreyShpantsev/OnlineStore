using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplicationTechSale.HelperServices
{
    public class UserValidator : IUserValidator<User>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            List<IdentityError> errors = new List<IdentityError>();
            string avoidValidationCode = ApplicationConstantsProvider.AvoidValidationCode();

            if (user.UserName == user.Email)
            {
                errors.Add(new IdentityError
                {
                    Description = "Имя пользователя и адрес электронной почты не должны совпадать"
                });
            }

            if (user.Email.Contains(avoidValidationCode))
            {
                user.Email = user.Email.Replace(avoidValidationCode, string.Empty);
            }
            else
            {
                User existingEmail = manager.FindByEmailAsync(user.Email).Result;
                if (existingEmail != null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Данный Email уже используется"
                    });
                }
            }

            if (user.UserName.Contains(avoidValidationCode))
            {
                user.UserName = user.UserName.Replace(avoidValidationCode, string.Empty);
            }
            else
            {
                User existingUserName = manager.FindByNameAsync(user.UserName).Result;
                if (existingUserName != null)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Имя пользователя занято"
                    });
                }
            }

            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}