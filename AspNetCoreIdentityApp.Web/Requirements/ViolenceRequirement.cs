using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ViolenceRequirement : IAuthorizationRequirement
    {
        public int TresholdAge { get; set; }
    }

    public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
        {
            var hasExhangeExpireClaim = context.User.HasClaim(x => x.Type == "birthday");
            if (!hasExhangeExpireClaim)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var birthdayClaim = context.User.FindFirst("birthday");


            var today = DateTime.Now;
            var birthDate = Convert.ToDateTime(birthdayClaim.Value); 
            var age = today.Year - birthDate.Year;

            //artık yıl hesaplama 
            if (birthDate > today.AddYears(-age)) age--;



            if (requirement.TresholdAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }


            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
