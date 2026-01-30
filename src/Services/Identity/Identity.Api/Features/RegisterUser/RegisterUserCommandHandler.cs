using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Identity.Api.Exceptions;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Api.Features.RegisterUser
{   
    public record RegisterUserCommand(string FirstName,
                                      string LastName, 
                                      string Username, 
                                      string Email,
                                      string Password, 
                                      string ConfirmPassword, 
                                      int PassportNumber) : ICommand<RegisterUserResult>;
    public record RegisterUserResult(Guid Id, 
                                     string FirstName, 
                                     string LastName, 
                                     string Username, 
                                     int PassportNumber);

    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please enter the password");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Please enter the confirmation password");

            RuleFor(x => x).Custom((x, context) =>
            {
                if (x.Password != x.ConfirmPassword)
                {
                    context.AddFailure(nameof(x.Password), "Passwords should match");
                }
            });

            RuleFor(x => x.Username).NotEmpty().WithMessage("Please enter the username");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please enter the first name");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Please enter the last name");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please enter the last email")
                .EmailAddress().WithMessage("A valid email is required");
        }
    }
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterUserResult>
    {
        private readonly UserManager<User> _userManager;

        public RegisterUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Username,
                Email = request.Email,
                PasswordHash = request.Password,
                PassPortNumber = request.PassportNumber
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);
            if (!identityResult.Succeeded)
            {
                throw new RegisterIdentityUserException(string.Join(',', identityResult.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, BuildingBlocks.Roles.Roles.User);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user); // Obrisi ako rola nije uspela 
                throw new RegisterIdentityUserException(string.Join(',', roleResult.Errors.Select(e => e.Description)));
            }

            return new RegisterUserResult(user.Id, 
                                          user.FirstName, 
                                          user.LastName,
                                          user.UserName, 
                                          user.PassPortNumber);
        }
    }
}
