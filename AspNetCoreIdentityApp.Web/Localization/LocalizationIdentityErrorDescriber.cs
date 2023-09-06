using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Localization
{
    public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError()
            {
                Code = "DuplicationUserName",
                Description = $"Bu {userName} daha önce başka bir kullanıcı tarafından alınmıştır."
            };

            //return base.DuplicateUserName(userName);
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = "DuplicationEmail",
                Description = $"Bu {email} başka bir kullanıcı tarafından alınmıştır."
            };



            //return base.DuplicateEmail(email);
        }
        public override IdentityError PasswordTooShort(int length)
        {
            return new()
            {
                Code = "PasswordTooShort",
                Description = $"Şifreniz en az 6 karakter içermesi gerek."
            };


            //return base.PasswordTooShort(length);
        }


    }
}
