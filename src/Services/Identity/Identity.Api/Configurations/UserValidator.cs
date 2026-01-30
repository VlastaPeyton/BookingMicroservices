using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Configurations
{
    public class UserValidator : IResourceOwnerPasswordValidator
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserValidator(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {   
            // Prvo, proveri postoji li username
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user is null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid credentials");
                return;
            }

            // Drugo, proveri password 
            var signIn = await _signInManager.PasswordSignInAsync(user,
                                                                  context.Password,
                                                                  isPersistent: false,
                                                                  lockoutOnFailure: false 
                                                                  );

            if (signIn.Succeeded)
            {
                var userId = user.Id.ToString();

                // Dodaj i role kao claims
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>{new Claim(ClaimTypes.NameIdentifier, userId),
                                             new Claim(ClaimTypes.Name, user.UserName ?? "")
                                            };

                // Dodaj svaku rolu kao claim
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                context.Result = new GrantValidationResult(
                    subject: userId,
                    authenticationMethod: "password",
                    claims: claims
                );

                return;
            }


            // context set to Failure ako wrong password ili user ne postoji
            context.Result = new GrantValidationResult(
                TokenRequestErrors.UnauthorizedClient, "Invalid Credentials");
        }
    }
}
